using System.Collections.Specialized;
using Sitecore;
using Sitecore.Diagnostics;
using ZipInfo.Model;

namespace ZipInfo.Providers
{
    public class ZipInfoProviderCache : ZipInfoProvider
    {
        private ZipCache _cache;

        private string _cacheMaxSize;
        private string _cacheObjectSize;
        private string _cacheName;

        public string CacheMaxSize => _cacheMaxSize;

        public string CacheObjectSize => _cacheObjectSize;

        public string CacheName => _cacheName;

        static ZipInfoProviderCache()
        {
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _cacheMaxSize = config["cacheMaxSize"];
            Assert.IsNotNullOrEmpty(_cacheMaxSize, "cacheMaxSize");

            _cacheObjectSize = config["cacheObjectSize"];
            Assert.IsNotNullOrEmpty(_cacheObjectSize, "cacheObjectSize");

            _cacheName = config["cacheName"];
            Assert.IsNotNullOrEmpty(_cacheName, "cacheName");

            var size = StringUtil.ParseSizeString(CacheMaxSize);
            Assert.IsTrue(size > 0, "CacheMaxSize");

            _cache = new ZipCache(_cacheName, StringUtil.ParseSizeString(CacheMaxSize), StringUtil.ParseSizeString(CacheObjectSize));
        }

        public override IZipCode Get(int zipCode)
        {
            var zip = _cache.GetZip(zipCode) as IZipCode;
            if (zip == null) return null;
            base.SetAbort();
            return zip;
        }

        public void Set(IZipCode zipCode)
        {
            if (!_cache.Contains(zipCode.Zip)) _cache.SetZip(zipCode);
        }

        public override string Reload(bool force)
        {
            return null;
        }

        public override void Wipe()
        {
            _cache.Clear();
        }

        public bool IsCached(int zipCode)
        {
            return _cache.Contains(zipCode);
        }


        public long GetCacheSize()
        {
            return _cache.InnerCache.Size;
        }

        public int GetCacheCount()
        {
            return _cache.InnerCache.Count;
        }


        public class ZipCache : Sitecore.Caching.CustomCache
        {
            private readonly long _objectSize;

            public ZipCache(string name, long maxSize, long objectSize) : base(name, maxSize)
            {
                this._objectSize = objectSize;
            }

            public void SetZip(IZipCode zip)
            {
                this.InnerCache.Add(zip.Zip, zip, _objectSize);
            }

            public object GetZip(int zip)
            {
                return this.InnerCache.MaxSize == 0 ? null : base.GetObject(zip);
            }

            public new void Clear()
            {
                if (this.InnerCache.MaxSize == 0)
                    return;

                base.InnerCache.Clear();
            }

            public bool Contains(int zip)
            {
                return InnerCache.ContainsKey(zip);
            }

        }
    }
}
