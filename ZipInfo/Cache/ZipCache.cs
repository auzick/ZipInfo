namespace ZipInfo.Cache
{
    public class ZipCache : Sitecore.Caching.CustomCache
    {
        public ZipCache(string name, long maxSize) : base(name, maxSize)
        {
        }

        public void SetZip(int zip, object record)
        {
            this.InnerCache.Add(zip, record, Configuration.Settings.Cache.ObjectSize);
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

    }
}
