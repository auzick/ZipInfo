using System;
using System.Configuration.Provider;

namespace ZipInfo.Providers
{
    public abstract class PropertyProvider : ProviderBase
    {
        protected PropertyProvider()
        {
        }

        public abstract string GetProperty(string property);

        public abstract bool SetProperty(string property, string value);

        public abstract DateTime LastUpdateDateProperty { get; set; }
    }
}
