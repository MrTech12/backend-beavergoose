﻿namespace FileMicroservice.DTOs
{
    public class SaveFileDTO
    {
        public string FileContent { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string AllowedDownloads { get; set; } = string.Empty;
    }
}
