using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Common.Helpers;
using Core.Storage.Models;
using Core.Storage.Repositories;
using Lambda.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda.GetTransaction
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

            context.Logger.LogInformation($"Get transaction with id: {transactionId}");

            var transaction = await _transactionRepository.GetTransactionAsync(transactionId);

            if (transaction == null)
                return FunctionHelper.CreateAPIGatewayProxyResponse(StatusCodes.Status204NoContent);

            return FunctionHelper.CreateAPIGatewayProxyResponse(StatusCodes.Status200OK, transaction);
        }
    }
}