using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core.Storage.Models;
using Core.Storage.Repositories;
using Lambda.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

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
            if (apigProxyEvent.PathParameters == null || !apigProxyEvent.PathParameters.ContainsKey(nameof(TransactionDto.TransactionId)))
                return GetAPIGatewayProxyResponse(StatusCodes.Status400BadRequest, new { ErrorMessage = $"{nameof(TransactionDto.TransactionId)} must be in path." });

            var transactionId = apigProxyEvent.PathParameters[nameof(TransactionDto.TransactionId)];

            context.Logger.LogInformation($"Get transaction with id: {transactionId}");

            var transaction = await _transactionRepository.GetTransactionAsync(transactionId);

            if (transaction == null)
                return GetAPIGatewayProxyResponse(StatusCodes.Status204NoContent, String.Empty);

            return GetAPIGatewayProxyResponse(200, transaction);
        }

        private APIGatewayProxyResponse GetAPIGatewayProxyResponse(int statusCode, object body) => new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(body),
            StatusCode = statusCode,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}