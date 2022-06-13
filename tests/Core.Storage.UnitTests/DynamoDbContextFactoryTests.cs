using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Moq;
using Xunit;

namespace Core.Storage.UnitTests
{
    public class DynamoDbContextFactoryTests
    {
        [Fact]
        public void Same_Instance_Of_IDynamoDBContext_Returned()
        {
            // Arrange
            var credentials = new BasicAWSCredentials("accessKey", "secretKey");
            var factory = new DynamoDBContextFactory(new AmazonDynamoDBClient(credentials, RegionEndpoint.EUCentral1));

            // Act && Assert
            Assert.Equal(factory.GetDynamoDBContext(), factory.GetDynamoDBContext());
        }

        [Fact]
        public void IDynamoDBContext_NotNull()
        {
            // Arrange
            var credentials = new BasicAWSCredentials("accessKey", "secretKey");
            var factory = new DynamoDBContextFactory(new AmazonDynamoDBClient(credentials, RegionEndpoint.EUCentral1));

            // Act && Assert
            Assert.NotNull(factory.GetDynamoDBContext());
        }
    }
}