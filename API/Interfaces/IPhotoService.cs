
using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        //ImageuploadResult will return a public id
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        //We can then use that public id to delete a photo
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}