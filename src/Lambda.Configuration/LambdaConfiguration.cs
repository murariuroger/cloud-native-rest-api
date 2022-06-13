using Amazon.DynamoDBv2;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Core.Storage.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.Configuration
{
    public static class LambdaConfiguration
    {
        private static IServiceProvider _services;
        public static IServiceProvider Services 
        { 
            get 
            {
                if (_services == null)
                    _services = GetConfiguredServices();
                return _services; 
            } 
        }

        private static IServiceProvider GetConfiguredServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(_ => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build());
            serviceCollection.AddStorage();

            return serviceCollection.BuildServiceProvider();    
        }
    }
}