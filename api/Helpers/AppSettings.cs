namespace dp.api.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
    }
    public class AwsSettings
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string UserPoolClientId { get; set; }
        public string UserPoolId { get; set; }
    }

}