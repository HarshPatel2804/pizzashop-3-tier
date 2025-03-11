using Microsoft.AspNetCore.Http;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class ImageService : IImageService
{
    public string GiveImagePath(IFormFile ProfileImage)
    {
        if(ProfileImage == null) return null;
        var fileGuid = Guid.NewGuid().ToString();
        
        // Define the upload folder path
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images" , "uploads");

        // Ensure the directory exists
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Get the file extension
        var fileExtension = Path.GetExtension(ProfileImage.FileName);
        var filePath = Path.Combine(uploadsFolder, fileGuid + fileExtension);

        // Save the file to the server
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
             ProfileImage.CopyToAsync(fileStream);
        }

        // Optionally, store the image filename or GUID in your database
        string path = fileGuid + fileExtension;  
        return path;
    }

}
