using Sitecore.Analytics.Tracking;
using ZipInfo.Model;

namespace ZipInfo
{
    public static class ContactLocationExtensions
    {
        public static SimpleZipCode GetZipInfo(this ContactLocation location)
        {
            int zip;
            if (!int.TryParse(location.PostalCode, out zip))
                return null;
            return (SimpleZipCode)ZipInfoManager.Get(zip);
        }

    }
}
