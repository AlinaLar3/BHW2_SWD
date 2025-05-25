
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using FileAnalysisService.Data;

namespace FileAnalysisService.Services
{
    public class TextAnalyzer : IAnalysisService
    {
        private readonly AnalysAppDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public TextAnalyzer(AnalysAppDbContext db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }

        public AnalysResult WordAnalysis(Guid fileId)
        {
            if (_db.Files.Any(f => f.FileId == fileId && f.AnalysResults != null))
            {
                var existingResult = _db.Files.Single(f => f.FileId == fileId).AnalysResults;
                return new AnalysResult()
                {
                    WordCount = existingResult.WordCount,
                    ParagraphCount = existingResult.ParagraphCount,
                    SymbolCount = existingResult.SymbolCount
                };
            }

            var client = _httpClientFactory.CreateClient();
            string text = GetFile(fileId, client); // Получение содержимого файла из FileStoringService
            var wordCount = Regex.Matches(text, @"\b\w+\b").Count;
            var paragraphCount = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Length; 
            var symbolCount = text.Count(char.IsLetter);
            
            var fileEntry = _db.Files.SingleOrDefault(f => f.FileId == fileId); // Поиск существующей записи файла или создание новой
            if (fileEntry == null)
            {
                fileEntry = new AnalysisResult { FileId = fileId };
                _db.Files.Add(fileEntry);
            }

            // Инициализация AnalysResults, если null, и обновление значений
            if (fileEntry.AnalysResults == null)
            {
                fileEntry.AnalysResults = new AnalysResult();
            }
            fileEntry.AnalysResults.WordCount = wordCount;
            fileEntry.AnalysResults.ParagraphCount = paragraphCount;
            fileEntry.AnalysResults.SymbolCount = symbolCount;

            _db.SaveChanges(); 

            return new AnalysResult() 
            {
                WordCount = wordCount,
                ParagraphCount = paragraphCount,
                SymbolCount = symbolCount
            };
        }

        private string GetFile(Guid fileId, HttpClient client)
        {
            var response = client.GetAsync($"http://file-storage:8001/file/get/{fileId}").Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve file");

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
