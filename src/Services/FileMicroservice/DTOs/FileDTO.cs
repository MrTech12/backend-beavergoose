namespace FileMicroservice.DTOs
{
    public class FileDTO
    {
        public string FileName { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public int AllowedDownloads { get; set; }
    }
}
