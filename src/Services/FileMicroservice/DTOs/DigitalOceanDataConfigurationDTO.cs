namespace FileMicroservice.DTOs
{
    public class DigitalOceanDataConfigurationDTO
    {
        public string DOServiceURL { get; set; }
        public string DOBucketName { get; set; }
        public string DOAccessKey { get; set; }
        public string DOSecretAccessKey { get; set; }
    }
}
