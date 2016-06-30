using MongoDB.Bson;

namespace ZipInfo.Model
{
    public class MongoProperty: IProperty
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
