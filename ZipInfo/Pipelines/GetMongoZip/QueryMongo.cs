using ZipInfo.Model;

namespace ZipInfo.Pipelines.GetMongoZip
{
    public class QueryMongo : GetMongoZipProcessorBase
    {
        public override void Process(GetMongoZipPipelineArgs args)
        {
            var query = GetZipQuery<SimpleZipCode>(args.ZipCode);
            args.ZipData = args.ZipCodesCollection.FindOneAs<SimpleZipCode>(query);
        }

    }
}
