using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Shared.Helpers;

public static class ImageUpload
{
    public static async Task<ImageUploadResult> CloudinaryImageUploadAsync(string imagePath, CloudinaryModel cloudinaryModel)
    {

        Account account = new Account
            (
              cloudinaryModel.CloudinaryUsername,
              cloudinaryModel.CloudinaryApiKey,
              cloudinaryModel.CloudinarySecreteKey
              );

        Cloudinary cloudinary = new Cloudinary(account);

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(@imagePath),

        };

        Task<ImageUploadResult> imageUploadTask = cloudinary.UploadAsync(uploadParams);


        var uploadResult = await imageUploadTask;

        return uploadResult;

    }
}
