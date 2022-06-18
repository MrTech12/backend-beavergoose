namespace LinkMicroservice.DTOs
{
    public class FileDTO
    {
        public string FileName { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public int AllowedDownloads { get; set; }
    }
}
