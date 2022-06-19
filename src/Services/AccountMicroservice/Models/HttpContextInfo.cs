namespace AccountMicroservice.Models
{
    public class HttpContextInfo
    {
        public string Host { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public string Scheme { get; set; } = string.Empty;
    }
}
