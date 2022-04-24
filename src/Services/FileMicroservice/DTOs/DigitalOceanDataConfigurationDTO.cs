namespace FileMicroservice.DTOs
{
    public class DigitalOceanDataConfigurationDTO
    {
        public string DOServiceURL { get; set; } = string.Empty;
        public string DOBucketName { get; set; } = string.Empty;
        public string DOAccessKey { get; set; } = string.Empty;
        public string DOSecretAccessKey { get; set; } = string.Empty;
    }
}
