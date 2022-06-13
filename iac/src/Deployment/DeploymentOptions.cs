using Amazon.CDK;
using System.Collections.Generic;

namespace IaC.Environments
{
    public static class DeploymentOptions
    {
        private static readonly Dictionary<string, Environment> Environments = new()
        {
            { "dev", new Environment { Account = "570298684443", Region = "eu-west-1" } },
            { "qa", new Environment { Account = "570298684443", Region = "eu-west-1" } },
            // Don't do this, prod must be in a separate account!
            { "prod", new Environment { Account = "570298684443", Region = "eu-west-1" } } 
        };

        public static Environment GetEnvironment(string envName)
        {
            if (envName.Contains("dev"))
                return Environments["dev"];

            return Environments[envName];
        }
    }
}
