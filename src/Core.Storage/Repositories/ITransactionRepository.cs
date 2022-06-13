using Core.Storage.Models;

namespace Core.Storage.Repositories
{
    public interface ITransactionRepository
    {
        Task UpsertAsync(TransactionDto transaction);
        Task<TransactionDto> GetTransactionAsync(string transactionId);
        Task DeleteTransactionAsync(string transactionId);
        Task<List<TransactionDto>> GetUserTransactionsAsync(string userEmail, DateTime start, DateTime end);
    }
}
