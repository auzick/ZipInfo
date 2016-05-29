using MongoDB.Bson;

namespace ZipInfo.Providers.Mongo
{
    public class ZipCode : IZipCode
    {
        public ObjectId Id { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int TimezoneOffset { get; set; }
        public bool ParticipatesInDst { get; set; }
    }
}
