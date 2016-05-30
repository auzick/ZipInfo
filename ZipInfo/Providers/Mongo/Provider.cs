using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.StringExtensions;
using Sitecore.Xml;
using ZipInfo.Configuration;

namespace ZipInfo.Providers.Mongo
{
    public class Provider : IZipInfoProvider
    {
        // This provider uses mongo to store the zip code info, using data from a csv file available from https://boutell.com/zipcodes/.

        // Thanks to Anders Laub Christoffersen for his blog post:
        // https://laubplusco.net/working-custom-mongodb-collections-sitecore-8-using-webapi/

        // Sample connection string for the zipcode collection
        //       <add name="zipinfo" connectionString="mongodb://localhost:27017/zipinfo" />
        // To maintain a separate zip database for each site sharing a common Mongo server, change
        // change the connection string e.g. connectionString="mongodb://localhost:27017/mysite_zipinfo"

        // When implementing another MongoDB-based provider using a different csv-based data source,
        // it might be simplest to inherit from this class and override the LoadRecord() method.


        public Provider()
        {
            InitializeCollections();
        }

        public IEnumerable<IZipCode> GetAll()
        {
            return _zipCodesCollection.FindAllAs<ZipCode>();
        }

        public IZipCode Get(int zipCode)
        {
            return _zipCodesCollection.FindOneAs<ZipCode>(GetZipQuery(zipCode));
        }

        public bool Set(IZipCode zipCode)
        {
            return _zipCodesCollection.Insert(zipCode).Ok;
        }

        public string Reload(bool force)
        {
            if (force || ShouldReload())
            {
                var file = GetSourceFilePath();
                if (string.IsNullOrWhiteSpace(file))
                    return "ZipInfo: Zip code file not found.";
                return Load(file, true);
            }
            return "ZipInfo: The data is current.";
        }

        // -------------------------------------------------------------------------------------------------------
        #region Mongo provider specific implementation.

        protected enum DatabaseProperty
        {
            DataLastUpdated
        }

        internal const string ConnectionStringName = "zipinfo";
        internal const string DataCollectionName = "zipinfo.zipcodes";
        private MongoCollection _zipCodesCollection;
        internal const string PropertiesCollectionName = "zipinfo.properties";
        private MongoCollection _propertiesCollection;

        protected static string FileNameSetting => XmlUtil.GetAttribute("location", Settings.ProviderConfig.SelectSingleNode("zipDataFile"));

        protected void InitializeCollections()
        {
            var connectionString =
                global::Sitecore.Configuration.Settings.GetConnectionString(ConnectionStringName);
            _zipCodesCollection = GetCollection(connectionString, DataCollectionName);
            Assert.IsNotNull(_zipCodesCollection, "_zipCodesCollection");
            _propertiesCollection = GetCollection(connectionString, PropertiesCollectionName);
            Assert.IsNotNull(_propertiesCollection, "_propertiesCollection");
        }


        protected string Load(string path, bool skipFirst)
        {
            var watch = new Stopwatch();
            Wipe();
            using (var csv = new CsvFileReader(path))
            {
                // Ignoring the first line
                if (skipFirst) csv.ReadLine();
                var line = csv.ReadLine();
                int linesRead = 0;
                watch.Start();
                while (line != null)
                {
                    line = csv.ReadLine();
                    if (line == null || line.Count != 7)
                        continue;
                    linesRead++;
                    IZipCode zip = LoadRecord(line);
                    Set(zip);
                }
                LastUpdateDateProperty = File.GetLastWriteTime(path);
                watch.Stop();
                var result = "ZipInfo: loaded {0} lines in {1} ms".FormatWith(linesRead,
                    watch.ElapsedMilliseconds.ToString());
                Log.Info(result, this);
                return result;
            }
        }

        protected IZipCode LoadRecord(List<string> fields)
        {
            var zip = new ZipCode
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
            var lastUpdated = LastUpdateDateProperty;
            var sourceFileDate = GetSourceFileDate();
            Log.Info("ZipInfo: File date is {0} {1}; last imported {2} {3}".FormatWith(
                sourceFileDate.ToShortDateString(),
                sourceFileDate.ToShortTimeString(),
                lastUpdated.ToShortDateString(),
                lastUpdated.ToShortTimeString()
                ), this);
            return sourceFileDate > lastUpdated;
        }

        protected void Wipe()
        {
            _zipCodesCollection.Drop();
        }


        private static IMongoQuery GetZipQuery(int zip)
        {
            return Query<ZipCode>.EQ(c => c.Zip, zip);
        }

        private static MongoCollection GetCollection(string connectionString, string collectionName)
        {
            var url = new MongoUrl(connectionString);
            return new MongoClient(url).GetServer().GetDatabase(url.DatabaseName).GetCollection(collectionName);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Properties can managed in the zipinfo.properties mongo collection

        private string GetProperty(DatabaseProperty property)
        {
            try
            {
                var val = _propertiesCollection.FindOneAs<MongoProperty>(Query<MongoProperty>.EQ(c => c.Name, property.ToString()));
                return val?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool SetProperty(DatabaseProperty property, string value)
        {
            var existing = _propertiesCollection.FindOneAs<MongoProperty>(Query<MongoProperty>.EQ(c => c.Name, property.ToString()));
            if (existing != null)
            {
                var query = Query<MongoProperty>.EQ(p => p.Name, property.ToString());
                return _propertiesCollection.Update(query, new UpdateBuilder().Set("Value", value)).Ok;
            }
            else
            {
                var prop = new MongoProperty() { Name = property.ToString(), Value = value };
                return _propertiesCollection.Insert(prop).Ok;
            }
        }

        private DateTime LastUpdateDateProperty
        {
            get
            {
                DateTime lastUpdated;
                if (!DateTime.TryParse(GetProperty(DatabaseProperty.DataLastUpdated), out lastUpdated))
                {
                    lastUpdated = DateTime.MinValue;
                }
                return lastUpdated;
            }
            set
            {
                SetProperty(DatabaseProperty.DataLastUpdated, value.ToUniversalTime().ToString("O"));
            }
        }

        private DateTime GetSourceFileDate()
        {
            var file = GetSourceFilePath();
            if (string.IsNullOrWhiteSpace(file))
                return DateTime.MaxValue;
            return TimeZoneInfo.ConvertTime(File.GetLastWriteTimeUtc(file), TimeZoneInfo.Local);
        }

        private string GetSourceFilePath()
        {
            var request = System.Web.HttpContext.Current.Request;
            string fileName = FileNameSetting;
            if (File.Exists(fileName)) return fileName;

            fileName = request.MapPath("~/App_Data/{0}".FormatWith(fileName));
            if (File.Exists(fileName)) return fileName;

            fileName = "{0}{1}".FormatWith(Sitecore.StringUtil.EnsurePostfix('\\', Sitecore.Configuration.Settings.DataFolder), fileName);
            if (File.Exists(fileName)) return fileName;

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
            var v = (bool)false;
            bool.TryParse(value, out v);
            return v;
        }

        #endregion

    }
}
