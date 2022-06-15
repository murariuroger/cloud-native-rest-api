using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

namespace Core.Common.Helpers
{
    public static class FunctionHelper
    {
        public static APIGatewayProxyResponse CreateAPIGatewayProxyResponse(int statusCode, object body) => new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(body),
            StatusCode = statusCode,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };

        public static APIGatewayProxyResponse CreateAPIGatewayProxyResponse(int statusCode) => new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}
