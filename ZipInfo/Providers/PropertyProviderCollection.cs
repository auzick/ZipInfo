using System.Configuration.Provider;
using Sitecore.Diagnostics;

namespace ZipInfo.Providers
{
    public class PropertyProviderCollection : ProviderCollection
    {
        public virtual void Add(PropertyProvider provider)
        {
            Assert.ArgumentNotNull(provider, "provider");
            base.Add(provider);
        }

        public override void Add(ProviderBase provider)
        {
            Assert.ArgumentNotNull(provider, "provider");
            var provider2 = provider as PropertyProvider;
            var message = "The provider type passed to PropertyProviderCollection is not assignable to PropertyProvider. Actual type: {0}";
            Assert.IsNotNull(provider2, message, new object[]{provider.GetType().FullName});
            this.Add(provider2);
        }

        public virtual PropertyProvider this[string name]
        {
            get
            {
                Assert.ArgumentNotNull(name, "name");
                return (base[name] as PropertyProvider);
            }
        }
    }
}
