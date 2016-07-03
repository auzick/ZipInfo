using System.Runtime.CompilerServices;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace ZipInfo.Pipelines.GetSourceFile
{
    public class GetSourceFilePipeline
    {
        public static GetSourceFilePipelineArgs Run(string configLocation)
        {
            var args = new GetSourceFilePipelineArgs() {ConfigLocation = configLocation};
            CorePipeline.Run("zipCode.getSourceFilePath", args);
            return args;
        }
    }
}
