using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

namespace Core.Common.Extensions
{
    public static class APIGatewayProxyRequestExtension
    {
        public static T DeserializeBody<T>(this APIGatewayProxyRequest request, ILambdaLogger logger)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(request.Body);
            }
            catch (Exception ex)
            {
                logger.LogError($"Request body cannot be deserialized to {nameof(T)}. Request body:{request.Body}. Error: {ex.Message}");
                return default(T);
            }
        }
    }
}
