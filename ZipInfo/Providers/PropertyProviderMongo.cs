using System;
using System.Collections.Specialized;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Sitecore.Diagnostics;
using ZipInfo.Model;

namespace ZipInfo.Providers
{
    public class PropertyProviderMongo : PropertyProvider
    {

        private MongoCollection _propertiesCollection;

        private string _connectionString;
        private string _collectionName;

        public string ConnectionString => _connectionString;

        public string CollectionName => _collectionName;

        static PropertyProviderMongo()
        {
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _connectionString = config["connectionString"];
            Assert.IsNotNullOrEmpty(_connectionString, "connectionString");

            _collectionName = config["collectionName"];
            Assert.IsNotNullOrEmpty(_collectionName, "collectionName");

            var connectionString = global::Sitecore.Configuration.Settings.GetConnectionString(ConnectionString);
            var url = new MongoUrl(connectionString);
            _propertiesCollection = new MongoClient(url)
                .GetServer()
                .GetDatabase(url.DatabaseName)
                .GetCollection(CollectionName);
            Assert.IsNotNull(_propertiesCollection, "_propertiesCollection");
        }

        public override string GetProperty(string property)
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

        public override bool SetProperty(string property, string value)
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

        public override DateTime LastUpdateDateProperty
        {
            get
            {
                DateTime lastUpdated;
                if (!DateTime.TryParse(PropertyManager.Provider.GetProperty("DataLastUpdated"), out lastUpdated))
                {
                    lastUpdated = DateTime.MinValue;
                }
                return lastUpdated;
            }
            set
            {
                PropertyManager.Provider.SetProperty("DataLastUpdated", value.ToUniversalTime().ToString("O"));
            }
        }
    }
}
