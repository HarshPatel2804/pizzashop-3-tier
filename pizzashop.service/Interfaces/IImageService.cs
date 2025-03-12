using Microsoft.AspNetCore.Http;

namespace pizzashop.service.Interfaces;

public interface IImageService
{
    Task<string> GiveImagePath(IFormFile ProfileImage);
}
