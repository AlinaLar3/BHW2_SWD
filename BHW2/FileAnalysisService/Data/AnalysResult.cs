using System;
using System.Collections.Generic;

namespace FileAnalysisService.Data
{
    public class AnalysResult
    {
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int SymbolCount { get; set; }
    }
}