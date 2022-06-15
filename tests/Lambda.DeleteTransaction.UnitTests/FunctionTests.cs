using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Storage.Models;
using Core.Storage.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Lambda.DeleteTransaction.UnitTests
{
    public class FunctionTests
    {
        private readonly Mock<ILambdaLogger> _loggerMock;
        private readonly Mock<ILambdaContext> _lambdaContextMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Function _sut;
        public FunctionTests()
        {
            _loggerMock = new Mock<ILambdaLogger>();
            _lambdaContextMock = new Mock<ILambdaContext>();
            _lambdaContextMock
                .Setup(_ => _.Logger)
                .Returns(_loggerMock.Object);
            _transactionRepositoryMock = new Mock<ITransactionRepository>();

            _sut = new(_transactionRepositoryMock.Object);
        }

        [Fact]
        public async void If_code_throws_Function_throws()
        {
            // Arrange
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { "TransactionId", "123456" } }
            };

            _transactionRepositoryMock
                .Setup(_ => _.DeleteTransactionAsync(It.IsAny<string>()))
                .Returns(() => throw new Exception("Some exception"));

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(async () => await _sut.FunctionHandler(request, _lambdaContextMock.Object));
        }

        [Fact]
        public async void If_Successfully_Function_Returns_204()
        {
            // Arrange
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { "TransactionId", "123456" } }
            };

            // Act
            var res = await _sut.FunctionHandler(request, _lambdaContextMock.Object);

            // Assert
            Assert.Equal(StatusCodes.Status204NoContent, res.StatusCode);
        }
    }
}