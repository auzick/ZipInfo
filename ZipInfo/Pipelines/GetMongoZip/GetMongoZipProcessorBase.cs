using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Sitecore.Diagnostics;
using ZipInfo.Model;

namespace ZipInfo.Pipelines.GetMongoZip
{
    public abstract class GetMongoZipProcessorBase
    {
        public abstract void Process(GetMongoZipPipelineArgs args);

        public virtual IMongoQuery GetZipQuery<T>(int zipCode) where T : IZipCode
        {
            Assert.IsTrue(zipCode > -1, "ZipCode not specified");
            return Query<T>.EQ(c => c.Zip, zipCode);
        }

    }
}
