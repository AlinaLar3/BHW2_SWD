
using System;
using System.ComponentModel.DataAnnotations;

namespace FileAnalysisService.Data
{
    public class AnalysisResult
    {
        [Key] 
        public Guid FileId { get; set; }

        [Required] 
        public AnalysResult AnalysResults { get; set; } = null!; 
    }
}
