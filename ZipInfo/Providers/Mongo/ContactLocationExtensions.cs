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



    }
}
