using Sitecore.Configuration;
using ZipInfo.Providers;

namespace ZipInfo
{
    public class PropertyManager
    {
        private static readonly ProviderHelper<PropertyProvider, PropertyProviderCollection> Helper;

        static PropertyManager()
        {
            Helper = new ProviderHelper<PropertyProvider, PropertyProviderCollection>("zipInfoProperties");
        }

        public static PropertyProvider Provider
        {
            get { return Helper.Provider; }
        }
    }
}
