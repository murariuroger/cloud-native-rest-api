using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Storage.Models;
using Core.Storage.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Lambda.PartialUpdateTransaction
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
            _transactionRepositoryMock
                .Setup(_ => _.GetTransactionAsync(It.IsAny<string>()))
                .ReturnsAsync(new TransactionDto() { UserEmail = "test@domain.com" });

            _sut = new(_transactionRepositoryMock.Object);
        }

        [Fact]
        public async void If_code_throws_Function_throws()
        {
            // Arrange
            var request = new APIGatewayProxyRequest()
            {
                Body = JsonSerializer.Serialize(new TransactionDto()),
                PathParameters = new Dictionary<string, string> { { "UserEmail", "test@test.com" } }
            };

            _transactionRepositoryMock
                .Setup(_ => _.UpsertAsync(It.IsAny<TransactionDto>()))
                .Returns(() => throw new Exception("Some exception"));

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(async () => await _sut.FunctionHandler(request, _lambdaContextMock.Object));
        }

        [Fact]
        public async void If_Body_Cannot_Be_Deserialized_Function_Returns_400()
        {
            // Arrange
            var request = new APIGatewayProxyRequest() 
            { 
                Body = "",
                PathParameters = new Dictionary<string, string> { { "TransactionId", "123456" } }
            };

            // Act
            var res = await _sut.FunctionHandler(request, _lambdaContextMock.Object);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
        }

        [Fact]
        public async void If_Transaction_Doesnt_Exists_Function_Returns_400()
        {
            // Arrange
            var request = new APIGatewayProxyRequest()
            {
                Body = "",
                PathParameters = new Dictionary<string, string> { { "TransactionId", "123456" } }
            };
            _transactionRepositoryMock
                .Setup(_ => _.GetTransactionAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var res = await _sut.FunctionHandler(request, _lambdaContextMock.Object);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
        }

        [Fact]
        public async void If_Successfuly_Function_Returns_200()
        {
            // Arrange
            var request = new APIGatewayProxyRequest()
            {
                Body = JsonSerializer.Serialize(new TransactionDto
                {
                    UserEmail = "test@test.com"
                }),
                PathParameters = new Dictionary<string, string> { { "TransactionId", "123456" } }
            };

            // Act
            var res = await _sut.FunctionHandler(request, _lambdaContextMock.Object);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }
    }
}