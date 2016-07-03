using System.IO;

namespace ZipInfo.Pipelines.GetSourceFile
{
    public class CheckAbsolutePath: GetSourceFileProcessorBase
    {
        public override void Process(GetSourceFilePipelineArgs args)
        {
            if (File.Exists(args.ConfigLocation))
            {
                args.Path = args.ConfigLocation;
                args.AbortPipeline();
            }

        }
    }
}
