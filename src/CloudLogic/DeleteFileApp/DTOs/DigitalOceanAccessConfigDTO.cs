namespace DeleteFileApp.DTOs
{
    public class DigitalOceanAccessConfigDTO
    {
        public string ServiceURL { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
    }
}
