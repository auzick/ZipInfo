using System;
using System.Linq;
using Sitecore.Analytics.Tracking;

namespace ZipInfo.Providers.Mongo
{
    public static class ContactLocationExtensions
    {
        public static ZipCode GetZipInfo(this ContactLocation location)
        {
            int zip;
            if (!int.TryParse(location.PostalCode, out zip))
                return null;
            return (ZipCode)ZipInfoManager.Get(zip);
        }

        //public static int GetUtcOffset(this ContactLocation location)
        //{
        //    int zip;
        //    if (!int.TryParse(location.PostalCode, out zip))
        //        return (int)0;
        //    return ((ZipCode) ZipInfoManager.Get(zip)).TimezoneOffset;
        //}

        //public static bool GetParticipatesInDst(this ContactLocation location)
        //{
        //    int zip;
        //    if (!int.TryParse(location.PostalCode, out zip))
        //        return true;
        //    return ((ZipCode)ZipInfoManager.Get(zip)).ParticipatesInDst;
        //}

        //public static DateTime GetLocalDateTime(this ContactLocation location)
        //{
        //    int zip;
        //    if (!int.TryParse(location.PostalCode, out zip))
        //        return DateTime.MinValue;
        //    var info = (ZipCode)ZipInfoManager.Get(zip);
        //    if (info == null)
        //        return DateTime.MinValue;
        //    return info.LocalDateTime;
        //}

    }
}
