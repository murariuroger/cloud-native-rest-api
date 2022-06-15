using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Storage.Models;
using Core.Storage.Repositories;
using Lambda.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Core.Common.Extensions;
using Core.Common.Helpers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda.UpdateTransaction
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
            var transaction = apigProxyEvent.DeserializeBody<TransactionDto>(context.Logger);
            if (transaction == null)
                return FunctionHelper.CreateAPIGatewayProxyResponse(StatusCodes.Status400BadRequest);

            transaction.TransactionId = apigProxyEvent.PathParameters[nameof(TransactionDto.TransactionId)];

            context.Logger.LogInformation($"Updating transaction {transaction.TransactionId}");

            await _transactionRepository.UpsertAsync(transaction);

            return FunctionHelper.CreateAPIGatewayProxyResponse(StatusCodes.Status200OK);
        }
    }
}