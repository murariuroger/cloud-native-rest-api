# Cloud native REST API
A cloud native REST API using C# for both runtime code and for IaC.

ToDo: Include diagram

# Running locally
Prerequisites:
- [CDK Setup](https://docs.aws.amazon.com/cdk/v2/guide/work-with.html#work-with-prerequisites)
- [NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

Our Lambdas are using DynamoDB and we will use the temporary stacks in dev environment for debugging. Once feature development is completed, stack can be deleted.

Follow the next steps in order to run the lambdas locally:
- Synthesize CDK stack ```cdk ls -c environment=dev_<user|feature>```
- Deploy ```cdk deploy --all --require-approval never -c environment=dev_<user|feature>```
- Deployed stack output contains __AccessKey__ and __SecretKey__, which are short lived and only created for dev env for debugging purposes in order to gain permission to real DynamoDB service. 
Please save the following as environment variables:

| Environment variable name| Value|
| :---:| :---: |
|AWS_Local__RunLocally|true|
|AWS_Local__AccessKey| < value from cfn output > |
|AWS_Local__SecretKey| < value from cfn output > |
|AWS_Local__Region| < value from cfn output > |
|DynamoDB__Transactions__TableName| < value from cfn output > |
- To debug please use __Lambdas.RunningLocally__ UnitTest project.

Delete stack from dev environment:
- ```cdk destroy -c environment=dev_<user|feature>```
> Please note that for dev env DynamoDB table will be removed.

# Deployments
TBD