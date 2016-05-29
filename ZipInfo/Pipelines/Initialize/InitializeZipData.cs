using Sitecore.Pipelines;

namespace ZipInfo.Pipelines.Initialize
{
    public class InitializeZipData
    {
        public void Process(PipelineArgs args)
        {
            ZipInfoManager.Provider.Reload(false);
        }

    }
}
