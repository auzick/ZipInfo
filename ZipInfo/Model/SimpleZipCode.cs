using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ZipInfo.Model
{
    public class SimpleZipCode : IZipCode
    {
        [BsonId]
        public int Zip { get; set; }

        public string City { get; set; }

        public string State { get; set; }
        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public int TimezoneOffset { get; set; }

        public bool ParticipatesInDst { get; set; }

        [BsonIgnore]
        public DateTime LocalDateTime
        {
            get
            {
                var isDst = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local));
                return DateTime.UtcNow.AddHours(TimezoneOffset).AddHours(isDst && ParticipatesInDst ? 1 : 0);
            }
        }
    }
}
