using Sitecore.Configuration;
using ZipInfo.Model;
using ZipInfo.Providers;

namespace ZipInfo
{
    public static class ZipInfoManager
    {

        private static readonly ProviderHelper<ZipInfoProvider, ZipInfoProviderCollection> Helper;

        static ZipInfoManager()
        {
            Helper = new ProviderHelper<ZipInfoProvider, ZipInfoProviderCollection>("zipInfoData");
        }

        public static ZipInfoProviderCollection Providers => Helper.Providers;

        public static ZipInfoProviderCache CacheProvider => Helper.Providers["cache"] as ZipInfoProviderCache;

        public static T Get<T>(int zipCode) where T : IZipCode
        {
            var zip = default(T);
            foreach (ZipInfoProvider provider in Helper.Providers)
            {
                zip = (T)provider.Get(zipCode);
                if (provider.IsAborted) break;
            }
            if (zip != null)
            {
                CacheProvider.Set(zip);
            }
            return zip;
        }

        public static IZipCode Get(int zipCode)
        {
            return Get<IZipCode>(zipCode);
        }
 
    }
}