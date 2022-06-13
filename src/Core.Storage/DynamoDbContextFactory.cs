using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace Core.Storage
{
    internal class DynamoDBContextFactory : IDynamoDBContextFactory
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private IDynamoDBContext _dynamoDBContext;
        public DynamoDBContextFactory(AmazonDynamoDBClient dynamoDBClient)
        {
            _dynamoDBClient = dynamoDBClient ?? throw new ArgumentNullException(nameof(dynamoDBClient));
        }

        public IDynamoDBContext GetDynamoDBContext()
        {
            if (_dynamoDBContext != null)
            {
                return _dynamoDBContext;
            }

            _dynamoDBContext = new DynamoDBContext(_dynamoDBClient);
            return _dynamoDBContext;
        }
    }
}
