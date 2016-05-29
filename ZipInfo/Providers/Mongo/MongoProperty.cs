using MongoDB.Bson;

namespace ZipInfo.Providers.Mongo
{
    public class MongoProperty
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
