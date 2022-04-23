using System.ComponentModel.DataAnnotations;

namespace LinkMicroservice.Entities
{
    public class Link
    {
        [Key]
        public int ID { get; set; }
        public string Address { get; set; }
        public string FileName { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public int AllowedDownloads { get; set; }
    }
}
