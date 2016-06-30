using System.Configuration.Provider;
using Sitecore.Diagnostics;

namespace ZipInfo.Providers
{
    public class ZipInfoProviderCollection : ProviderCollection
    {
        public virtual void Add(ZipInfoProvider provider)
        {
            Assert.ArgumentNotNull(provider, "provider");
            base.Add(provider);
        }

        public override void Add(ProviderBase provider)
        {
            Assert.ArgumentNotNull(provider, "provider");
            var provider2 = provider as ZipInfoProvider;
            var message = "The provider type passed to ZipInfoProviderCollection is not assignable to ZipInfoProvider. Actual type: {0}";
            Assert.IsNotNull(provider2, message, new object[] { provider.GetType().FullName });
            this.Add(provider2);
        }

        public virtual ZipInfoProvider this[string name]
        {
            get
            {
                Assert.ArgumentNotNull(name, "name");
                return (base[name] as ZipInfoProvider);
            }
        }

    }
}