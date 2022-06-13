using Amazon.CDK;
using Constructs;
using IaC.Constructs;

namespace IaC.Stacks
{
    internal class MainStack : Stack
    {
        public MainStack(Construct scope, string id, MainStackProps props) : base(scope, id, props)
        {
            new CloudNativeRestApiConstruct(this, "CloudNativeRestApiConstruct", new()
            {
                Environment = props.EnvironmentName
            });
        }
    }
}
