namespace FileMicroservice.DTOs
{
    public class UploadFileDTO
    {
        public IFormFile File { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string AllowedDownloads { get; set; } = string.Empty;
    }
}
