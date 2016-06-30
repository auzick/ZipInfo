using Sitecore.Pipelines;

namespace ZipInfo.Pipelines.Initialize
{
    public class InitializeMongoZipData
    {
        public void Process(PipelineArgs args)
        {
            ZipInfoManager.Providers["mongo"].Reload(false);
        }

    }
}
