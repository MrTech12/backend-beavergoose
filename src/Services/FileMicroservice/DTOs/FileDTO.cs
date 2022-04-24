namespace FileMicroservice.DTOs
{
    public class FileDTO
    {
        public string FileName { get; set; } = string.Empty;
        public string SenderID { get; set; } = string.Empty;
        public string ReceiverID { get; set; } = string.Empty;
        public int AllowedDownloads { get; set; }
    }
}
