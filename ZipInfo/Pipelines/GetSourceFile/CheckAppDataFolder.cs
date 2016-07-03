using System.IO;
using System.Web;
using Sitecore.StringExtensions;

namespace ZipInfo.Pipelines.GetSourceFile
{
    public class CheckAppDataFolder : GetSourceFileProcessorBase
    {
        public override void Process(GetSourceFilePipelineArgs args)
        {
            var loc = HttpContext.Current.Request.MapPath("~/App_Data/{0}".FormatWith(args.ConfigLocation));
            if (File.Exists(loc))
            {
                args.Path = loc;
                args.AbortPipeline();
            }
        }
    }
}
