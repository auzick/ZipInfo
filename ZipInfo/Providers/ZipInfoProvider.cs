using System.Configuration.Provider;
using ZipInfo.Model;

namespace ZipInfo.Providers
{
    public abstract class ZipInfoProvider : ProviderBase
    {
        protected ZipInfoProvider()
        {
        }

        public abstract IZipCode Get(int zipCode);

        public abstract string Reload(bool force);

        public abstract void Wipe();

        private bool _isAborted = false;
        public bool IsAborted => _isAborted;
        public void SetAbort()
        {
            _isAborted = true;
        }
    }
}
