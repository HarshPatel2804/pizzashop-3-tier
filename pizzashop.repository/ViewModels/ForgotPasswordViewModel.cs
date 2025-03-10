using System.ComponentModel.DataAnnotations;

namespace pizzashop.repository.ViewModels;
public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Email")]
    public string email { get; set; }
}