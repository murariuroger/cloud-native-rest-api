using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Common.Extensions;
using Core.Common.Helpers;
using Core.Storage.Models;
using Core.Storage.Repositories;
using Lambda.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda.PartialUpdateTransaction
{
    public class Function
    {
        private readonly ITransactionRepository _transactionRepository;

        public Function()
        {
            _transactionRepository = LambdaConfiguration.Services.GetRequiredService<ITransactionRepository>();
        }

        public Function(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            var transactionId = apigProxyEvent.PathParameters[nameof(TransactionDto.TransactionId)];
            var transactionRequest = apigProxyEvent.DeserializeBody<TransactionDto>(context.Logger);

            var existingTransaction = await _transactionRepository.GetTransactionAsync(transactionId);
            if (existingTransaction == null || transactionRequest == null)
                return FunctionHelper.CreateAPIGatewayProxyResponse(StatusCodes.Status400BadRequest);

            if(!string.IsNullOrWhiteSpace(transactionRequest.UserEmail))
                existingTransaction.UserEmail = transactionRequest.UserEmail;
            if (!string.IsNullOrWhiteSpace(transactionRequest.FailedReason))
                existingTransaction.FailedReason = transactionRequest.FailedReason;
            if (!string.IsNullOrWhiteSpace(transactionRequest.OrderSide))
                existingTransaction.OrderSide = transactionRequest.OrderSide;
            if (!string.IsNullOrWhiteSpace(transactionRequest.Status))
                existingTransaction.Status = transactionRequest.Status;
            if (!string.IsNullOrWhiteSpace(transactionRequest.TimeInForce))
                existingTransaction.TimeInForce = transactionRequest.TimeInForce;
            if (!string.IsNullOrWhiteSpace(transactionRequest.Type))
                existingTransaction.Type = transactionRequest.Type;
            if (transactionRequest.Date != DateTime.MinValue)
                existingTransaction.Date = transactionRequest.Date;
            if (transactionRequest.OrigQty != default(decimal))
                existingTransaction.OrigQty = transactionRequest.OrigQty;
            if (transactionRequest.Price != default(decimal))
                existingTransaction.Price= transactionRequest.Price;
            if (transactionRequest.UsdtAmount != default(decimal))
                existingTransaction.UsdtAmount = transactionRequest.UsdtAmount;

            await _transactionRepository.UpsertAsync(existingTransaction);

            return FunctionHelper.CreateAPIGatewayProxyResponse(StatusCodes.Status200OK);
        }
    }
}