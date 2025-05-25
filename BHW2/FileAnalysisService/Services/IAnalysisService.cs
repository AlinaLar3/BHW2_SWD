using System;
using System.Net.Http; // Используем IHttpClientFactory
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using FileAnalysisService.Data;

namespace FileAnalysisService.Services
{
    public interface IAnalysisService
    {
        AnalysResult WordAnalysis(Guid fileId);
    }
}