using System.ComponentModel.DataAnnotations;

namespace LinkMicroservice.DBMigration.Entities
{
    public class Link
    {
        [Key]
        public int ID { get; set; }
        public string Address { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string SenderID { get; set; } = string.Empty;
        public string ReceiverID { get; set; } = string.Empty;
        public int AllowedDownloads { get; set; }
    }
}
