using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Core.Storage.Models;
using IaC.DynamoDb;
using IaC.Environments;
using System.Collections.Generic;

namespace IaC.Constructs
{
    public class CloudNativeRestApiConstruct : Construct
    {
        public CloudNativeRestApiConstruct(Construct scope, string id, CloudNativeRestApiConstructProps props) : base(scope, id)
        {
            #region DynamoDb
            var transactionTable = new TransactionsTable(this, "Transactions", new TableProps
            {
                TableName = $"Transactions_{props.Environment}",
                BillingMode = BillingMode.PROVISIONED,
                PartitionKey = new Attribute
                {
                    Name = nameof(TransactionDto.TransactionId),
                    Type = AttributeType.STRING
                },
                Encryption = TableEncryption.AWS_MANAGED,
                ReadCapacity = 10,
                WriteCapacity = 10
            });
            #endregion

            #region Running locally
            if (props.Environment.Contains("dev"))
            {
                transactionTable.ApplyRemovalPolicy(RemovalPolicy.DESTROY);
                var devUser = new User(this, $"short-lived-user", default);
                var accessKey = new CfnAccessKey(this, $"short-lived-access-key", new CfnAccessKeyProps { UserName = devUser.UserName });
                transactionTable.GrantReadWriteData(devUser);

                new CfnOutput(this, "AWS_Local__AccessKey", new CfnOutputProps { Value = accessKey.Ref });
                new CfnOutput(this, "AWS_Local__SecretKey", new CfnOutputProps { Value = accessKey.AttrSecretAccessKey });
                new CfnOutput(this, "AWS_Local__Region", new CfnOutputProps { Value = DeploymentOptions.GetEnvironment("dev").Region });
                new CfnOutput(this, "DynamoDB__Transactions__TableName", new CfnOutputProps { Value = transactionTable.TableName });
            }
            #endregion

            #region Lambdas
            var lambdaGetTransactionProps = new FunctionProps
            {
                FunctionName= $"GetTransaction-{props.Environment}",
                Description = "Lambda Function - Get Transaction",
                Handler = "Lambda.GetTransaction::Lambda.GetTransaction.Function::FunctionHandler",
                Code = Code.FromAsset("./Assets/Lambda.GetTransaction"),
                Runtime = Runtime.DOTNET_6,
                Timeout = Duration.Seconds(90),
                LogRetention = Amazon.CDK.AWS.Logs.RetentionDays.TWO_MONTHS,
                MemorySize = 2500,
                Tracing = Tracing.ACTIVE,
                Environment = new Dictionary<string, string>()
                {
                    { "AWS_Local__RunLocally", "false" },
                    { "DynamoDB__Transactions__TableName", transactionTable.TableName }
                }
            };
            var getTransactionLambda = new Function(this, "GetTransaction", lambdaGetTransactionProps);
            transactionTable.GrantReadData(getTransactionLambda);

            var lambdaAddTransactionProps = new FunctionProps
            {
                FunctionName = $"AddTransaction-{props.Environment}",
                Description = "Lambda Function - Add Transaction",
                Handler = "Lambda.AddTransaction::Lambda.AddTransaction.Function::FunctionHandler",
                Code = Code.FromAsset("./Assets/Lambda.AddTransaction"),
                Runtime = Runtime.DOTNET_6,
                Timeout = Duration.Seconds(90),
                LogRetention = Amazon.CDK.AWS.Logs.RetentionDays.TWO_MONTHS,
                MemorySize = 2500,
                Tracing = Tracing.ACTIVE,
                Environment = new Dictionary<string, string>()
                {
                    { "AWS_Local__RunLocally", "false" },
                    { "DynamoDB__Transactions__TableName", transactionTable.TableName }
                }
            };
            var addTransactionLambda = new Function(this, "AddTransaction", lambdaAddTransactionProps);
            transactionTable.GrantWriteData(addTransactionLambda);

            var lambdaDeleteTransactionProps = new FunctionProps
            {
                FunctionName = $"DeleteTransaction-{props.Environment}",
                Description = "Lambda Function - Delete Transaction",
                Handler = "Lambda.DeleteTransaction::Lambda.DeleteTransaction.Function::FunctionHandler",
                Code = Code.FromAsset("./Assets/Lambda.DeleteTransaction"),
                Runtime = Runtime.DOTNET_6,
                Timeout = Duration.Seconds(90),
                LogRetention = Amazon.CDK.AWS.Logs.RetentionDays.TWO_MONTHS,
                MemorySize = 2500,
                Tracing = Tracing.ACTIVE,
                Environment = new Dictionary<string, string>()
                {
                    { "AWS_Local__RunLocally", "false" },
                    { "DynamoDB__Transactions__TableName", transactionTable.TableName }
                }
            };
            var deleteTransactionLambda = new Function(this, "DeleteTransaction", lambdaDeleteTransactionProps);
            transactionTable.GrantWriteData(deleteTransactionLambda);

            var lambdaUpdateTransactionProps = new FunctionProps
            {
                FunctionName = $"UpdateTransaction-{props.Environment}",
                Description = "Lambda Function - Update Transaction",
                Handler = "Lambda.UpdateTransaction::Lambda.UpdateTransaction.Function::FunctionHandler",
                Code = Code.FromAsset("./Assets/Lambda.UpdateTransaction"),
                Runtime = Runtime.DOTNET_6,
                Timeout = Duration.Seconds(90),
                LogRetention = Amazon.CDK.AWS.Logs.RetentionDays.TWO_MONTHS,
                MemorySize = 2500,
                Tracing = Tracing.ACTIVE,
                Environment = new Dictionary<string, string>()
                {
                    { "AWS_Local__RunLocally", "false" },
                    { "DynamoDB__Transactions__TableName", transactionTable.TableName }
                }
            };
            var updateTransactionLambda = new Function(this, "UpdateTransaction", lambdaUpdateTransactionProps);
            transactionTable.GrantWriteData(updateTransactionLambda);
            #endregion

            #region ApiGateway
            var restApiProps = new RestApiProps
            {
                RestApiName = $"TransactionsRestAPI-{props.Environment}",
                Description = $"Cloud native REST API.",
                DeployOptions = new StageOptions()
                {
                    StageName = props.Environment
                }
            };

            var restApi = new RestApi(this, "CloudNativeRestApi", restApiProps);

            var transactionsResource = restApi.Root.AddResource("transactions");
            transactionsResource.AddMethod("POST", new LambdaIntegration(addTransactionLambda));

            var transactionIdResource = transactionsResource.AddResource("{" + nameof(TransactionDto.TransactionId) + "}");
            transactionIdResource.AddMethod("GET", new LambdaIntegration(getTransactionLambda));
            transactionIdResource.AddMethod("DELETE", new LambdaIntegration(deleteTransactionLambda));
            transactionIdResource.AddMethod("PUT", new LambdaIntegration(updateTransactionLambda));

            #endregion

            #region Tags
            Tags.Of(this).Add("team", "alpha");
            Tags.Of(this).Add("service", "transactions-service");
            Tags.Of(this).Add("sensitive-info", "false");
            Tags.Of(this).Add("env", props.Environment);
            #endregion
        }
    }
}
