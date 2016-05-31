using System.Collections.Generic;
using ZipInfo.Cache;
using ZipInfo.Providers;

namespace ZipInfo
{
    public static class ZipInfoManager
    {
        public static IZipInfoProvider Provider { get; }

        private static readonly ZipCache Cache = new ZipCache("ZipInfo", Configuration.Settings.Cache.MaxSize);

        static ZipInfoManager()
        {
            Provider = Configuration.Settings.GetProvider();
        }

        public static IEnumerable<IZipCode> GetAll()
        {
            return Provider.GetAll();
        }

        public static IZipCode Get(int zipCode)
        {
            var fromCache = Cache.GetZip(zipCode);
            if (fromCache != null) return (IZipCode)fromCache;
            var fromProvider = Provider.Get(zipCode);
            Cache.SetZip(zipCode, fromProvider);
            return fromProvider;
        }

        public static bool Set(IZipCode zipCode)
        {
            return Provider.Set(zipCode);
        }

        public static string Reload(bool force, bool wipe)
        {
            Cache.Clear();
            if (wipe) Provider.Wipe();
            return Provider.Reload(force || wipe);
        }


        public static bool IsCached(int zipCode)
        {
            return Cache.GetZip(zipCode) != null;
        }

        public static long GetCacheSize()
        {
            return Cache.InnerCache.Size;
        }

        public static int GetCacheCount()
        {
            return Cache.InnerCache.Count;
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
