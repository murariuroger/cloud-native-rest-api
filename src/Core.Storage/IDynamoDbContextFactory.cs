using Amazon.DynamoDBv2.DataModel;

namespace Core.Storage
{
    public interface IDynamoDBContextFactory
    {
        public IDynamoDBContext GetDynamoDBContext();
    }
}
