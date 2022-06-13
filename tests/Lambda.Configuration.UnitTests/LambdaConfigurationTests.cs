using Amazon.DynamoDBv2;
using Core.Storage;
using Core.Storage.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lambda.Configuration.UnitTests
{
    public class LambdaConfigurationTests
    {
        [Fact]
        public void IConfiguration_Can_Be_Resolved()
        {
            Assert.NotNull(LambdaConfiguration.Services.GetRequiredService<IConfiguration>());
        }

        [Fact]
        public void ITransactionRepository_Can_Be_Resolved()
        {
            Assert.NotNull(LambdaConfiguration.Services.GetRequiredService<ITransactionRepository>());
        }

        [Fact]
        public void AmazonDynamoDBClient_Can_Be_Resolved()
        {
            Assert.NotNull(LambdaConfiguration.Services.GetRequiredService<AmazonDynamoDBClient>());
        }

        [Fact]
        public void IDynamoDBContextFactory_Can_Be_Resolved()
        {
            Assert.NotNull(LambdaConfiguration.Services.GetRequiredService<IDynamoDBContextFactory>());
        }

        [Fact]
        public void Same_Instance_Of_ServiceProvider_Returned()
        {
            var sp = LambdaConfiguration.Services;
            Assert.Equal(sp, LambdaConfiguration.Services);
        }
    }
}