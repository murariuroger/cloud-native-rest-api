using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Core.Storage.Constants;
using Core.Storage.Models;
using Core.Storage.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Core.Storage.UnitTests
{
    public class TransactionRepositoryTests
    {
        private readonly ITransactionRepository _sut;
        private readonly Mock<IDynamoDBContext> _dynamoDbContextMock;
        private const string DynamoDbTableName = nameof(DynamoDbTableName);
        public TransactionRepositoryTests()
        {
            _dynamoDbContextMock = new Mock<IDynamoDBContext>();

            var dynamoDbContextFactoryMock = new Mock<IDynamoDBContextFactory>();
            dynamoDbContextFactoryMock
                .Setup(_ => _.GetDynamoDBContext())
                .Returns(_dynamoDbContextMock.Object);

            var inMemorySettings = new Dictionary<string, string> { { "DynamoDB:Transactions:TableName", DynamoDbTableName } };
            var configuration = new ConfigurationBuilder()
                            .AddInMemoryCollection(inMemorySettings)
                            .Build();

            _sut = new TransactionRepository(dynamoDbContextFactoryMock.Object, configuration);
        }

        [Fact]
        public async void TableName_From_IConfiguration_Is_Used_For_GetTransaction()
        {
            // Act
            var res = await _sut.GetTransactionAsync("");

            // Assert
            _dynamoDbContextMock
                .Verify(_ => _.LoadAsync<TransactionDto>(
                    It.IsAny<string>(), 
                    It.Is<DynamoDBOperationConfig>(opC => opC.OverrideTableName.Equals(DynamoDbTableName)), 
                    default
                ),
                Times.Once);
        }

        [Fact]
        public async void TableName_From_IConfiguration_Is_Used_For_UpsertTransaction()
        {
            // Act
            await _sut.UpsertAsync(new TransactionDto());

            // Assert
            _dynamoDbContextMock
                .Verify(_ => _.SaveAsync<TransactionDto>(
                    It.IsAny<TransactionDto>(),
                    It.Is<DynamoDBOperationConfig>(opC => opC.OverrideTableName.Equals(DynamoDbTableName)),
                    default
                ),
                Times.Once);
        }

        [Fact]
        public async void TableName_From_IConfiguration_Is_Used_For_DeleteTransaction()
        {
            // Act
            await _sut.DeleteTransactionAsync("");

            // Assert
            _dynamoDbContextMock
                .Verify(_ => _.DeleteAsync<TransactionDto>(
                    It.IsAny<string>(),
                    It.Is<DynamoDBOperationConfig>(opC => opC.OverrideTableName.Equals(DynamoDbTableName)),
                    default
                ),
                Times.Once);
        }
    }
}
