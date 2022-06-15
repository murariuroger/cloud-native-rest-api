using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Storage.Models;
using Lambda.PartialUpdateTransaction;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Lambdas.RunningLocally
{
    public class PartialUpdateTransactionRunner
    {
        public class UpdateTransactionRunner
        {
            private readonly Function _function;
            private readonly Mock<ILambdaLogger> _loggerMock;
            private readonly Mock<ILambdaContext> _lambdaContextMock;
            public UpdateTransactionRunner()
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
                    PathParameters = new Dictionary<string, string> { { "TransactionId", "324fedr43" } },
                    Body = JsonSerializer.Serialize(new TransactionDto
                    {
                        UserEmail = "test@test.com"
                    })
                };

                var res = await _function.FunctionHandler(request, _lambdaContextMock.Object);

                Assert.NotNull(res);
            }
        }
    }
}
