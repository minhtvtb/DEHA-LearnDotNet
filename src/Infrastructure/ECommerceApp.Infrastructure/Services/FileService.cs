using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ECommerceApp.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null", nameof(file));

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = GetUniqueFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            filePath = filePath.TrimStart('/');
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                + "_" + Guid.NewGuid().ToString().Substring(0, 8)
                + Path.GetExtension(fileName);
        }
    }

    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        void DeleteFile(string filePath);
    }
} 