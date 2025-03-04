using System.ComponentModel.DataAnnotations;

namespace pizzashop.repository.ViewModels;

public class ChangePasswordViewModel
{
    [Required]
    public string? OldPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }

    public string? Email { get; set; }
    public string? Token { get; set; }
}
