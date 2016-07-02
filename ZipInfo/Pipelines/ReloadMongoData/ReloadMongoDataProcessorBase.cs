namespace ZipInfo.Pipelines.ReloadMongoData
{
    public abstract class ReloadMongoDataProcessorBase
    {
        public abstract void Process(ReloadMongoDataPipelineArgs args);

    }
}
