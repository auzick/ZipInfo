using System.Collections.Generic;
using System.Linq;
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

        public static T Get<T>(int zipCode)
        {
            IZipCode zip = null;
            foreach (ZipInfoProvider provider in Helper.Providers)
            {
                zip = provider.Get(zipCode);
                if (provider.IsAborted) break;
            }
            return (T)zip;

            //var zip =
            //    (from ZipInfoProvider provider in Helper.Providers select provider.Get(zipCode))
            //        .FirstOrDefault(z => z != null);
            //if (zip != null)
            //    CacheProvider.Set(zip);
            //return (T)zip;
        }

        public static IZipCode Get(int zipCode)
        {
            return Get<IZipCode>(zipCode);
        }
 
    }
}