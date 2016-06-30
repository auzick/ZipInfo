using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.StringExtensions;
using ZipInfo.Model;

namespace ZipInfo.Providers
{
    public class ZipInfoProviderMongo : ZipInfoProvider
    {
        // This provider uses mongo to store the zip code info, using data from a csv file available from https://boutell.com/zipcodes/.

        // Thanks to Anders Laub Christoffersen for his blog post:
        // https://laubplusco.net/working-custom-mongodb-collections-sitecore-8-using-webapi/

        // Thanks to Adam Conn for his blog post:
        // http://www.sitecore.net/da-dk/learn/blogs/technical-blogs/getting-to-know-sitecore/posts/2010/06/managing-temporary-data.aspx

        // Sample connection string for the zipcode collection
        //       <add name="zipinfo" connectionString="mongodb://localhost:27017/zipinfo" />
        // To maintain a separate zip database for each site sharing a common Mongo server, change
        // change the connection string e.g. connectionString="mongodb://localhost:27017/mysite_zipinfo"

        // When implementing another MongoDB-based provider using a different csv-based data source,
        // it might be simplest to inherit from this class and override the LoadRecord() method.

        private MongoCollection _zipCodesCollection;

        private string _dataFileLocation;
        private string _connectionString;
        private string _collectionName;

        public string DataFileLocation => _dataFileLocation;

        public string ConnectionString => _connectionString;

        public string CollectionName => _collectionName;


        static ZipInfoProviderMongo()
        {
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _dataFileLocation = config["dataFileLocation"];
            Assert.IsNotNullOrEmpty(_dataFileLocation, "dataFileLocation");

            _connectionString = config["connectionString"];
            Assert.IsNotNullOrEmpty(_dataFileLocation, "connectionString");

            _collectionName = config["collectionName"];
            Assert.IsNotNullOrEmpty(_dataFileLocation, "collectionName");

            var connectionString = global::Sitecore.Configuration.Settings.GetConnectionString(ConnectionString);
            _zipCodesCollection = GetCollection(connectionString, CollectionName);
            Assert.IsNotNull(_zipCodesCollection, "_zipCodesCollection");
        }

        public IEnumerable<IZipCode> GetAll()
        {
            return _zipCodesCollection.FindAllAs<SimpleZipCode>();
        }

        public override IZipCode Get(int zipCode)
        {
            return _zipCodesCollection.FindOneAs<SimpleZipCode>(GetZipQuery(zipCode));
        }

        public bool Set(IZipCode zipCode, out WriteConcernResult result)
        {
            var q = GetZipQuery(zipCode.Zip);
            result = _zipCodesCollection.Update(q, Update.Replace(zipCode), UpdateFlags.Upsert);
            return result.Ok;
        }

        public override string Reload(bool force)
        {
            if (force || ShouldReload())
            {
                var file = GetSourceFilePath();
                if (string.IsNullOrWhiteSpace(file))
                    return "ZipInfo: Zip code file not found.";
                return Load(file);
            }
            return "ZipInfo: The data is current.";
        }

        public override void Wipe()
        {
            _zipCodesCollection.Drop();
        }


        // -------------------------------------------------------------------------------------------------------
        #region Mongo provider specific implementation.

        protected string Load(string path)
        {
            var watch = new Stopwatch();
            using (var csv = new CsvFileReader(path))
            {
                var line = csv.ReadLine();
                var linesRead = (int)0;
                var recordsWritten = (int)0;
                var recordsUpdated = (int)0;
                var recordsFailed = (int)0;
                watch.Start();
                while (line != null)
                {
                    line = csv.ReadLine();
                    if (line == null || line.Count != 7)
                        continue;
                    linesRead++;
                    var zip = LoadRecord(line);
                    var q = GetZipQuery(zip.Zip);
                    var setResult = _zipCodesCollection.Update(q, Update.Replace(zip), UpdateFlags.Upsert);

                    if (setResult.UpdatedExisting)
                        recordsUpdated++;
                    else
                        recordsWritten++;
                    if (!setResult.Ok)
                        recordsFailed++;
                }
                PropertyManager.Provider.LastUpdateDateProperty = File.GetLastWriteTime(path);
                watch.Stop();
                var result = "ZipInfoProviderMongo: Read {0} lines, {1} new records, {2} updated records, {3} failed operations, in {4} ms".FormatWith(
                    linesRead,
                    recordsWritten,
                    recordsUpdated,
                    recordsFailed,
                    watch.ElapsedMilliseconds.ToString());
                Log.Info(result, this);
                return result;
            }
        }

        protected IZipCode LoadRecord(List<string> fields)
        {
            var zip = new SimpleZipCode
            {
                Zip = GetInt(fields[0]),
                City = fields[1],
                State = fields[2],
                Latitude = GetFloat(fields[3]),
                Longitude = GetFloat(fields[4]),
                TimezoneOffset = GetInt(fields[5]),
                ParticipatesInDst = GetBool(fields[6])
            };
            return zip;
        }

        protected bool ShouldReload()
        {
            var lastUpdated = PropertyManager.Provider.LastUpdateDateProperty;
            var sourceFileDate = GetSourceFileDate();
            Log.Info("ZipInfoProviderMongo: File date is {0} {1}; last imported {2} {3}".FormatWith(
                sourceFileDate.ToShortDateString(),
                sourceFileDate.ToShortTimeString(),
                lastUpdated.ToShortDateString(),
                lastUpdated.ToShortTimeString()
                ), this);
            return sourceFileDate > lastUpdated;
        }

        private static IMongoQuery GetZipQuery(int zip)
        {
            return Query<SimpleZipCode>.EQ(c => c.Zip, zip);
        }

        private static MongoCollection GetCollection(string connectionString, string collectionName)
        {
            var url = new MongoUrl(connectionString);
            return new MongoClient(url).GetServer().GetDatabase(url.DatabaseName).GetCollection(collectionName);
        }

        private DateTime GetSourceFileDate()
        {
            var file = GetSourceFilePath();
            return string.IsNullOrWhiteSpace(file) ? DateTime.MaxValue : TimeZoneInfo.ConvertTime(File.GetLastWriteTimeUtc(file), TimeZoneInfo.Local);
        }

        private string GetSourceFilePath()
        {
            if (File.Exists(DataFileLocation)) return DataFileLocation;

            var request = System.Web.HttpContext.Current.Request;
            var loc = string.Empty;

            loc = request.MapPath("~/App_Data/{0}".FormatWith(DataFileLocation));
            if (File.Exists(loc)) return loc;

            loc = "{0}{1}".FormatWith(Sitecore.StringUtil.EnsurePostfix('\\', Sitecore.Configuration.Settings.DataFolder), DataFileLocation);
            if (File.Exists(loc)) return loc;

            return null;
        }

        protected float GetFloat(string value)
        {
            var v = (float)0;
            float.TryParse(value, out v);
            return v;
        }

        protected int GetInt(string value)
        {
            var v = (int)0;
            int.TryParse(value, out v);
            return v;
        }

        protected bool GetBool(string value)
        {
            var v = false;
            if (bool.TryParse(value, out v))
                return v;
            int vi;
            if (int.TryParse(value, out vi))
                v = Convert.ToBoolean(vi);
            return v;
        }

        #endregion

    }
}
