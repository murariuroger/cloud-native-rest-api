using Amazon.DynamoDBv2;
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
            var dynamoDbClientMock = new Mock<AmazonDynamoDBClient>();
            var factory = new DynamoDBContextFactory(dynamoDbClientMock.Object);

            // Act && Assert
            Assert.Equal(factory.GetDynamoDBContext(), factory.GetDynamoDBContext());
        }

        [Fact]
        public void IDynamoDBContext_NotNull()
        {
            // Arrange
            var dynamoDbClientMock = new Mock<AmazonDynamoDBClient>();
            var factory = new DynamoDBContextFactory(dynamoDbClientMock.Object);

            // Act && Assert
            Assert.NotNull(factory.GetDynamoDBContext());
        }
    }
}