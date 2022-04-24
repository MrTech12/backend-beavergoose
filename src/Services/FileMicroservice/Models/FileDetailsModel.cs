namespace FileMicroservice.Models
{
    public class FileDetailsModel
    {
        public string SenderID { get; set; } = string.Empty;
        public string ReceiverID { get; set; } = string.Empty;
        public int AllowedDownloads { get; set; }
    }
}
