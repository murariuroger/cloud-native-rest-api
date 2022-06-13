using Amazon.CDK;

namespace IaC.Stacks
{
    internal class MainStackProps : StackProps
    {
        public string EnvironmentName { get; set; }
    }
}
