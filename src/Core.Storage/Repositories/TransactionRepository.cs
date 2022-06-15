using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Core.Storage.Constants;
using Core.Storage.Models;
using Microsoft.Extensions.Configuration;

namespace Core.Storage.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDynamoDBContext _dBContext;
        private readonly string _transactionsTableName;

        public TransactionRepository(IDynamoDBContextFactory dynamoDBContextFactory, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _transactionsTableName = configuration.GetValue<string>("DynamoDB:Transactions:TableName");
            _dBContext = dynamoDBContextFactory?.GetDynamoDBContext() ?? throw new ArgumentNullException(nameof(dynamoDBContextFactory));
        }

        public async Task DeleteTransactionAsync(string transactionId)
        {
            await _dBContext.DeleteAsync<TransactionDto>(transactionId, new DynamoDBOperationConfig() { OverrideTableName = _transactionsTableName });
        }

        public async Task<TransactionDto> GetTransactionAsync(string transactionId)
        {
            return await _dBContext.LoadAsync<TransactionDto>(transactionId, new DynamoDBOperationConfig(){ OverrideTableName = _transactionsTableName });
        }

        public async Task<List<TransactionDto>> GetUserTransactionsAsync(string userEmail, DateTime start, DateTime end)
        {
            var operationConfig = new DynamoDBOperationConfig
            {
                IndexName = DynamoDbConstants.TransactionsUserDateIndexName,
                OverrideTableName = _transactionsTableName
            };
            var sortKeyValues = new List<object> { start, end };

            return await _dBContext
                .QueryAsync<TransactionDto>(userEmail, QueryOperator.Between, sortKeyValues, operationConfig)
                .GetRemainingAsync();
        }

        public async Task UpsertAsync(TransactionDto transaction)
        {
            await _dBContext.SaveAsync(transaction, new DynamoDBOperationConfig() { OverrideTableName = _transactionsTableName });
        }
    }
}
