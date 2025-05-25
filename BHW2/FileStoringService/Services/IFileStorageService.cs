using FileStoringService.Data;
using System.Threading.Tasks;

namespace FileStoringService.Services
{
    public interface IFileStorageService
    {
        Guid StoreFile(IFormFile file);
        (Stream stream, string contentType, string fileName) RetrieveFile(Guid id); 
    }
}