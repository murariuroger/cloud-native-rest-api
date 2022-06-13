using Amazon.CDK;
using IaC.Environments;
using IaC.Stacks;

namespace Iac
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var targetedEnvironment = app.Node.TryGetContext("environment") as string;
            new MainStack(app, $"MainStack-{targetedEnvironment}", new MainStackProps
            {
                Env = DeploymentOptions.GetEnvironment(targetedEnvironment),
                EnvironmentName = targetedEnvironment,
                Description = "Cloud native REST API"
            });

            app.Synth();
        }
    }
}
