using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Storage.Models;
using Lambda.AddTransaction;
using Moq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Lambdas.RunningLocally
{
    public class AddTransactionRunner
    {
        private readonly Function _function;
        private readonly Mock<ILambdaLogger> _loggerMock;
        private readonly Mock<ILambdaContext> _lambdaContextMock;

        public AddTransactionRunner()
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
            var request = new APIGatewayProxyRequest()
            {
                Body = JsonSerializer.Serialize(new TransactionDto
                {
                    TransactionId = "324fedr43"
                })
            };

            var res = await _function.FunctionHandler(request, _lambdaContextMock.Object);

            Assert.NotNull(res);
        }
    }
}
