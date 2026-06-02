using Microsoft.AspNetCore.Http;

namespace HearMeStay.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadAccommodationImageAsync(IFormFile file);
        Task<string> UploadRoomImageAsync(IFormFile file);
    }
}
