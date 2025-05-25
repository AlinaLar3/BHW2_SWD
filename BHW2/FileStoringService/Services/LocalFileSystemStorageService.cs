
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using FileStoringService.Data;

namespace FileStoringService.Services
{
    public class LocalFileSystemStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly FileAppDbContext _db;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;
        
        public LocalFileSystemStorageService(
            FileAppDbContext db,
            FileExtensionContentTypeProvider contentTypeProvider,
            ILogger<LocalFileSystemStorageService> logger)
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            _db = db;
            _contentTypeProvider = contentTypeProvider;
            Directory.CreateDirectory(_basePath);
        }

        public Guid StoreFile(IFormFile file)
        {
            var id = Guid.NewGuid();
            var filePath = Path.Combine(_basePath, id.ToString());

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save file to disk.", ex);
            }
            var metadata = new FileMetadata
            {
                Id = id,
                FileName = file.FileName,
                StorageLocation = filePath
            };

            _db.FilesMetadata.Add(metadata);
            _db.SaveChanges();
            return id;
        }

        public (Stream stream, string contentType, string fileName) RetrieveFile(Guid id)
        {
            var fileEntry = _db.FilesMetadata.Find(id); 
            if (fileEntry == null)
            {
                throw new FileNotFoundException($"File with ID {id} not found in database.");
            }
            
            if (!System.IO.File.Exists(fileEntry.StorageLocation)) 
            {
                throw new FileNotFoundException($"Physical file not found at location: {fileEntry.StorageLocation}");
            }
            var stream = new FileStream(fileEntry.StorageLocation, FileMode.Open, FileAccess.Read, FileShare.Read); 
            var contentType = DetermineContentType(fileEntry.FileName); 
            return (stream, contentType, fileEntry.FileName);
        }

        private string DetermineContentType(string fileName)
        {
            if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
