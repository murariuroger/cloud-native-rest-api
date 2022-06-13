using Amazon.DynamoDBv2;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Core.Storage.Repositories;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Storage.Models.Options;
using Amazon;

namespace Core.Storage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddStorage(this IServiceCollection services)
        {
            services.AddSingleton<AmazonDynamoDBClient>(sp => 
            { 
                var configuration = sp.GetRequiredService<IConfiguration>();
                var awsOptions = configuration
                 .GetSection(AWSOptions.Section)
                 .Get<AWSOptions>();

                if (awsOptions.RunLocally)
                {
                    var credential = new BasicAWSCredentials(awsOptions.AccessKey, awsOptions.SecretKey);
                    return new AmazonDynamoDBClient(credential, RegionEndpoint.GetBySystemName(awsOptions.Region));
                }

                AWSSDKHandler.RegisterXRay<IAmazonDynamoDB>();
                return new AmazonDynamoDBClient();
            });
            services.AddSingleton<IDynamoDBContextFactory, DynamoDBContextFactory>();
            services.AddSingleton<ITransactionRepository, TransactionRepository>();
        }
    }
}
