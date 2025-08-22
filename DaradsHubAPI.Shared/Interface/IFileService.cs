using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Shared.Interface;
public interface IFileService
{
    Task<ImageUploadResult> AddPhoto(IFormFile file, string folderName);
}
