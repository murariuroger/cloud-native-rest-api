namespace Core.Storage.Models.Options
{
    internal class AWSOptions
    {
        public const string Section = "AWS_Local";
        public bool RunLocally { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
    }
}
