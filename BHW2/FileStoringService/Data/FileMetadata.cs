using System;
using System.ComponentModel.DataAnnotations;

namespace FileStoringService.Data
{
    public class FileMetadata
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string StorageLocation { get; set; }
    }
}