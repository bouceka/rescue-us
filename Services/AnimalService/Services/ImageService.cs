using AnimalService.Helpers;
using AnimalService.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace AnimalService.Services
{
    public class ImageService : IImageService
    {

        private readonly Cloudinary _cloudinary;

        public readonly IConfiguration Configuration;
        private CloudinarySettings _cloudinarySettings;
        public ImageService(IConfiguration configuration)
        {
            Configuration = configuration;
            _cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var acc = new Account
            (
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "rescue-us"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }

    }
}