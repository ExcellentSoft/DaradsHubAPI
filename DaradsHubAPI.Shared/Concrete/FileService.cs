using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DaradsHubAPI.Shared.Interface;
using DaradsHubAPI.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DaradsHubAPI.Shared.Concrete;
public class FileService : IFileService
{
    private readonly CloudSettings _options;
    private readonly Cloudinary _cloudinary;

    public FileService(IOptionsSnapshot<CloudSettings> options)
    {
        _options = options.Value;
        Account acc = new(
             _options.CloudName,
             _options.ApiKey,
             _options.ApiSecret);
        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhoto(IFormFile file, string folderName)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Folder = folderName
                };
                uploadResult = _cloudinary.Upload(uploadParams);
            }
        }
        return await Task.FromResult(uploadResult);
    }
}