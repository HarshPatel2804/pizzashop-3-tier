using System.ComponentModel.DataAnnotations;

namespace pizzashop.repository.ViewModels;

public class ChangePasswordViewModel
{
     [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must have at least 1 uppercase letter, 1 number, and 1 special character and must have at least 8 characters.")]   
         public string? OldPassword { get; set; }
         
     [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must have at least 1 uppercase letter, 1 number, and 1 special character and must have at least 8 characters.")]   
         public string? Password { get; set; }

     [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must have at least 1 uppercase letter, 1 number, and 1 special character and must have at least 8 characters.")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

    public string? Email { get; set; }
    public string? Token { get; set; }
}
