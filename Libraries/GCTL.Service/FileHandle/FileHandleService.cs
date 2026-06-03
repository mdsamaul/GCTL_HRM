using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using System;
using System.IO;
using System.Threading.Tasks;

namespace GCTL.Service.FileHandle
{
    public class FileHandleService : IFileHandle 
    {
        public async Task<string> SaveFileAsync(IFormFile file, string rootPath, string folderName, string customFileName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");

            if (string.IsNullOrEmpty(rootPath))
                throw new ArgumentException("Root path cannot be null or empty.");

            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentException("Folder name cannot be null or empty.");

            // Create the directory if it doesn't exist
            var uploadsFolder = Path.Combine(rootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Use custom file name if provided, otherwise generate a unique name
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = string.IsNullOrEmpty(customFileName)
                ? Guid.NewGuid().ToString() + fileExtension
                : customFileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + fileExtension;

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path to the file
            return $"/uploads/{folderName}/{fileName}";
        }
    }
}
