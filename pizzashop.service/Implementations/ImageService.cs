using Microsoft.AspNetCore.Http;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class ImageService : IImageService
{
    public async Task<string> GiveImagePath(IFormFile ProfileImage)
    {
        if(ProfileImage == null) return null;
        var fileGuid = Guid.NewGuid().ToString();
        
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images" , "uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileExtension = Path.GetExtension(ProfileImage.FileName);
        var filePath = Path.Combine(uploadsFolder, fileGuid + fileExtension);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
             await ProfileImage.CopyToAsync(fileStream);
        }

        string path = fileGuid + fileExtension;  
        return path;
    }

}
