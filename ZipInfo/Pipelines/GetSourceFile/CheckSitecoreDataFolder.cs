using System.IO;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.StringExtensions;

namespace ZipInfo.Pipelines.GetSourceFile
{
    public class CheckSitecoreDataFolder : GetSourceFileProcessorBase
    {
        public override void Process(GetSourceFilePipelineArgs args)
        {
            var loc =
                "{0}{1}".FormatWith(
                    StringUtil.EnsurePostfix('\\', Settings.DataFolder),
                    args.ConfigLocation);
            if (File.Exists(loc))
            {
                args.Path = loc;
                args.AbortPipeline();
            }
        }
    }
}