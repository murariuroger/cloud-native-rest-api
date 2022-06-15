using Amazon.CDK.Assertions;
using Core.Storage.Models;
using IaC.Constructs;
using System.Collections.Generic;
using Xunit;
using Match = Amazon.CDK.Assertions.Match;
using Stack = Amazon.CDK.Stack;

namespace IaC.UnitTests
{
    public class IaCTests
    {
        private readonly Stack _sut;
        private const string EnvName = "test";
        public IaCTests()
        {
            _sut = new Stack();

            var construct = new CloudNativeRestApiConstruct(_sut, "CloudNativeRestApiConstruct", new CloudNativeRestApiConstructProps()
            {
                Environment = EnvName
            });
        }

        [Fact]
        public void Lambda_GetTransaction_Created_And_Tags_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::Lambda::Function",
                new Dictionary<string, object>
                {
                    { "Handler", "Lambda.GetTransaction::Lambda.GetTransaction.Function::FunctionHandler" },
                    { "FunctionName", $"GetTransaction-{EnvName}" },
                    { "Runtime", "dotnet6" },
                    { "Tags",  TagsMatcher }
                });
        }

        [Fact]
        public void Lambda_AddTransaction_Created_And_Tags_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::Lambda::Function",
                new Dictionary<string, object>
                {
                    { "Handler", "Lambda.AddTransaction::Lambda.AddTransaction.Function::FunctionHandler" },
                    { "FunctionName", $"AddTransaction-{EnvName}" },
                    { "Runtime", "dotnet6" },
                    { "Tags",  TagsMatcher }
                });
        }

        [Fact]
        public void Lambda_DeleteTransaction_Created_And_Tags_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::Lambda::Function",
                new Dictionary<string, object>
                {
                    { "Handler", "Lambda.DeleteTransaction::Lambda.DeleteTransaction.Function::FunctionHandler" },
                    { "FunctionName", $"DeleteTransaction-{EnvName}" },
                    { "Runtime", "dotnet6" },
                    { "Tags",  TagsMatcher }
                });
        }

        [Fact]
        public void Lambda_UpdateTransaction_Created_And_Tags_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::Lambda::Function",
                new Dictionary<string, object>
                {
                    { "Handler", "Lambda.UpdateTransaction::Lambda.UpdateTransaction.Function::FunctionHandler" },
                    { "FunctionName", $"UpdateTransaction-{EnvName}" },
                    { "Runtime", "dotnet6" },
                    { "Tags",  TagsMatcher }
                });
        }

        [Fact]
        public void APIGatewayRestApi_Created_And_Tags_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::ApiGateway::RestApi",
                new Dictionary<string, object>
                {
                    { "Description", "Cloud native REST API." },
                    { "Name", $"TransactionsRestAPI-{EnvName}" },
                    { "Tags",  TagsMatcher }
                });
        }

        [Fact]
        public void APIGateway_GetTransaction_Resource_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::ApiGateway::Resource",
                new Dictionary<string, string>
                {
                    { "PathPart", "transactions"}
                });
        }

        [Fact]
        public void APIGateway_GetTransaction_Method_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::ApiGateway::Method",
                new Dictionary<string, string>
                {
                    { "HttpMethod", "GET"}
                });
        }

        [Fact]
        public void APIGateway_DeleteTransaction_Method_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::ApiGateway::Method",
                new Dictionary<string, string>
                {
                    { "HttpMethod", "DELETE"}
                });
        }

        [Fact]
        public void APIGateway_UpdateTransaction_Method_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::ApiGateway::Method",
                new Dictionary<string, string>
                {
                    { "HttpMethod", "PUT"}
                });
        }

        [Fact]
        public void APIGateway_AddTransaction_Resource_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::ApiGateway::Resource",
                new Dictionary<string, string>
                {
                    { "PathPart", "{" + nameof(TransactionDto.TransactionId) + "}"}
                });
        }

        [Fact]
        public void APIGateway_AddTransaction_Method_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::ApiGateway::Method",
                new Dictionary<string, string>
                {
                    { "HttpMethod", "POST"}
                });
        }

        [Fact]
        public void DynamoDb_Transactions_Table_Created()
        {
            var template = Template.FromStack(_sut);

            // Assert
            template.HasResourceProperties("AWS::DynamoDB::Table",
                new Dictionary<string, string>
                {
                    { "TableName", $"Transactions_{EnvName}"}
                });

        }

        private Matcher TagsMatcher => Match.ArrayWith(new object[]
            {
                new Dictionary<string, object>
                {
                    {"Key", "env" },
                    {"Value", EnvName }
                },
                new Dictionary<string, object>
                {
                    {"Key", "sensitive-info" },
                    {"Value", "false" }
                },
                new Dictionary<string, object>
                {
                    {"Key", "service" },
                    {"Value", "transactions-service" }
                },
                new Dictionary<string, object>
                {
                    {"Key", "team" },
                    {"Value", "alpha" }
                }
            });
    }
}