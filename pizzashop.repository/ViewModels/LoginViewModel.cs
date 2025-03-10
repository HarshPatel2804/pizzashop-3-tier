using System.ComponentModel.DataAnnotations;

namespace pizzashop.repository.ViewModels;
public class LoginViewModel
{
     [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Email")]
    public string Email { get; set; } 

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must have at least 1 uppercase letter, 1 number, and 1 special character and must have at least 8 characters.")]
    public string Password { get; set; } 

    public bool Remember { get; set; }
}