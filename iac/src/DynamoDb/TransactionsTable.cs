using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using Core.Storage.Constants;
using Core.Storage.Models;

namespace IaC.DynamoDb
{
    internal class TransactionsTable : Table
    {
        public TransactionsTable(Construct scope, string id, ITableProps props) : base(scope, id, props)
        {
            this.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = DynamoDbConstants.TransactionsUserDateIndexName,
                PartitionKey = new Attribute { Name = nameof(TransactionDto.UserEmail), Type = AttributeType.STRING },
                SortKey = new Attribute { Name = nameof(TransactionDto.Date), Type = AttributeType.STRING },
                ReadCapacity = 10,
                WriteCapacity = 10,
                ProjectionType = ProjectionType.ALL
            });

            this.AutoScaleReadCapacity(new EnableScalingProps { MinCapacity = 10, MaxCapacity = 1000 })
                .ScaleOnUtilization(new UtilizationScalingProps { TargetUtilizationPercent = 85 });

            this.AutoScaleWriteCapacity(new EnableScalingProps { MinCapacity = 10, MaxCapacity = 1000 })
                .ScaleOnUtilization(new UtilizationScalingProps { TargetUtilizationPercent = 85 });

            this.AutoScaleGlobalSecondaryIndexReadCapacity(DynamoDbConstants.TransactionsUserDateIndexName, new EnableScalingProps { MinCapacity = 10, MaxCapacity = 1000 })
                .ScaleOnUtilization(new UtilizationScalingProps { TargetUtilizationPercent = 85 });

            this.AutoScaleGlobalSecondaryIndexWriteCapacity(DynamoDbConstants.TransactionsUserDateIndexName, new EnableScalingProps { MinCapacity = 10, MaxCapacity = 1000 })
                .ScaleOnUtilization(new UtilizationScalingProps { TargetUtilizationPercent = 85 });
        }
    }
}
