namespace FileMicroservice.Models
{
    public class FileDetailsModel
    {
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public int AllowedDownloads { get; set; }
    }
}
