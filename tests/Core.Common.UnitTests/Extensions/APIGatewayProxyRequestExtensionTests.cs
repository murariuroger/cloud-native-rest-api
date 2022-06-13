using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Common.Extensions;
using Moq;
using Xunit;

namespace Core.Common.UnitTests.Extensions
{
    public class APIGatewayProxyRequestExtensionTests
    {
        private class Dto
        {
            public string SomeProperty { get; set; }
            public string OtherProperty { get; set; }
        }

        [Fact]
        public void If_Body_Cannot_Be_Deserialized_Returns_Null()
        {
            // Arrange
            var request = new APIGatewayProxyRequest
            {
                Body = ""
            };
            var loggerMock = new Mock<ILambdaLogger>();

            // Act
            var res = request.DeserializeBody<Dto>(loggerMock.Object);

            // Assert
            Assert.Null(res);
        }

        [Fact]
        public void If_Body_Cannot_Be_Deserialized_Error_Logged()
        {
            // Arrange
            var request = new APIGatewayProxyRequest
            {
                Body = ""
            };
            var loggerMock = new Mock<ILambdaLogger>();

            // Act
            var res = request.DeserializeBody<Dto>(loggerMock.Object);

            // Assert
            loggerMock.Verify(_ => _.LogError(It.Is<string>(m => m.Contains("Request body cannot be deserialized"))));
        }
    }
}
