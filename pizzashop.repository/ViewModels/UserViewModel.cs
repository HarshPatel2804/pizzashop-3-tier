using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.ViewModels;

public enum statustype
{
        Active = 0,
        Inactive = 1
}

public class UserViewModel
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
        [Remote(action: "CheckPhone", controller: "User", AdditionalFields = nameof(Id)
    , ErrorMessage = "User exist with this Phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [RegularExpression(@"^[A-Za-z0-9]{2,20}$",
                ErrorMessage = "First name can have only alphabates and numbers and can be of 2 to 20 characters long.")]
        [Remote(action: "CheckUsername", controller: "User", AdditionalFields = nameof(Id)
    , ErrorMessage = "Username is already taken.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
           ErrorMessage = "Password must have at least 1 uppercase letter, 1 number, and 1 special character and must have at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int Role { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email")]
        [Remote(action: "CheckEmail", controller: "User", AdditionalFields = nameof(Id)
    , ErrorMessage = "User exist with this Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Zipcode is required.")]
        [RegularExpression(@"^[0-9]{6}$",
                ErrorMessage = "Zipcode name can have only numbers and can be of 6 characters.")]
        public string? Zipcode { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public statustype status { get; set; }

        public string Profileimg { get; set; }
        public List<SelectListItem> Countries { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SelectListItem> Cities { get; set; }

        public List<SelectListItem> Roles { get; set; }

}
