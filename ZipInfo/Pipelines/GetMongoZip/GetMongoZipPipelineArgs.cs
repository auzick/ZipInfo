using MongoDB.Driver;
using Sitecore.Pipelines;
using ZipInfo.Model;

namespace ZipInfo.Pipelines.GetMongoZip
{
    public class GetMongoZipPipelineArgs : PipelineArgs 
    {
        public MongoCollection ZipCodesCollection { get; set; }

        public int ZipCode { get; set; }

        public IZipCode ZipData { get; set; }

        public GetMongoZipPipelineArgs(MongoCollection collection, int zipCode)
        {
            ZipCodesCollection = collection;
            ZipCode = zipCode;
        }

    }
}
