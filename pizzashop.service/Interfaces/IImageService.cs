using Microsoft.AspNetCore.Http;

namespace pizzashop.service.Interfaces;

public interface IImageService
{
    string GiveImagePath(IFormFile ProfileImage);
}
