using System.ComponentModel.DataAnnotations;

namespace pizzashop.repository.ViewModels;
public class LoginViewModel
{
     [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Email")]
    public string Email { get; set; } 

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } 

    public bool Remember { get; set; }
}