using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GCTL.Service.FileHandle
{
    public interface IFileHandle
    {
        Task<string> SaveFileAsync(IFormFile file, string rootPath, string folderName, string customFileName = null);
    }
}
