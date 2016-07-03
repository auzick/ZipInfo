using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics;

namespace ZipInfo.Pipelines.GetSourceFile
{
    public class ValidateConfig : GetSourceFileProcessorBase
    {
        public override void Process(GetSourceFilePipelineArgs args)
        {
            if (string.IsNullOrEmpty(args.ConfigLocation))
            {
                Log.Error("ZipInfo.Pipelines.GetSourceFile: Provider configuration \"dataFileLocation\" attribute is missing.", typeof (ValidateConfig));
                args.AbortPipeline();
            }
        }
    }
}
