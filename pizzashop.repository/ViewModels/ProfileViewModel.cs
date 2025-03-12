using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.ViewModels;

public class ProfileViewModel
{
   public int Id { get; set; }

[Required(ErrorMessage = "First Name is required.")]
    [RegularExpression(@"^[A-Za-z]{2,50}$",
            ErrorMessage = "First name can have only alphabates and can be of 2 to 50 characters long.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [RegularExpression(@"^[A-Za-z]{2,50}$",
            ErrorMessage = "Last name can have only alphabates and can be of 2 to 50 characters long.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Phone number must be of 10 digits.")]
    public string Phone { get; set; }
    [Required(ErrorMessage = "User Name is required.")]
    [RegularExpression(@"^[A-Za-z0-9]{2,20}$",
            ErrorMessage = "First name can have only alphabates and numbers and can be of 2 to 20 characters long.")]
    public string Username { get; set; }

    public string Rolename { get; set;}

    public string Email { get; set; }
    
    [Required(ErrorMessage = "Country is required.")]

    public int CountryId { get; set; }

    public string Profileimg {get; set;}

    [Required(ErrorMessage = "State is required.")]
    public int StateId { get; set; }

    [Required(ErrorMessage = "City is required.")]
    public int CityId { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public List<SelectListItem> Countries {get; set;}

     public List<SelectListItem> States {get; set;}

      public List<SelectListItem> Cities {get; set;}

    public List<SelectListItem> Roles {get; set;}

}
