using MongoDB.Driver;
using Sitecore.Pipelines;

namespace ZipInfo.Pipelines.GetMongoZip
{
    public static class GetMongoZipPipeline
    {
        public static GetMongoZipPipelineArgs Run(MongoCollection collection, int zipCode)
        {
            var args = new GetMongoZipPipelineArgs(collection, zipCode);
            CorePipeline.Run("zipCode.getMongoZip", args);
            return args;
        }

    }
}
