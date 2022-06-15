using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Lambda.DeleteTransaction;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Lambdas.RunningLocally
{
    public class DeleteTransactionRunner
    {

        private readonly Function _function;
        private readonly Mock<ILambdaLogger> _loggerMock;
        private readonly Mock<ILambdaContext> _lambdaContextMock;

        public DeleteTransactionRunner()
        {
            _loggerMock = new Mock<ILambdaLogger>();
            _lambdaContextMock = new Mock<ILambdaContext>();
            _lambdaContextMock
                .Setup(_ => _.Logger)
                .Returns(_loggerMock.Object);

            _function = new Function();
        }

        [Fact]
        public async Task Run()
        {
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { "TransactionId", "324fedr43" } }
            };

            var res = await _function.FunctionHandler(request, _lambdaContextMock.Object);

            Assert.NotNull(res);
        }
    }
}
