using Microsoft.AspNetCore.Mvc;

namespace FileMicroservice.Models
{
    public class FileDetailsModel
    {
        [FromHeader]
        public string SenderID { get; set; } = string.Empty;
        [FromHeader]
        public string ReceiverID { get; set; } = string.Empty;
        [FromHeader]
        public int AllowedDownloads { get; set; }
    }
}
