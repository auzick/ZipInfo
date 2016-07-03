using System;
using Sitecore.Pipelines;

namespace ZipInfo.Pipelines.GetSourceFile
{
    public class GetSourceFilePipelineArgs : PipelineArgs
    {
        public string ConfigLocation { get; set; }

        public string Path { get; set; }

        public GetSourceFilePipelineArgs()
        {
        }
    }
}
