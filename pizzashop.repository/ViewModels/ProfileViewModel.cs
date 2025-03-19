using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.ViewModels;

public class ProfileViewModel
{
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name can contain only alphabets.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name can contain only alphabets.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$",
                ErrorMessage = "Phone number must be of 10 digits.")]
        [Remote(action: "CheckPhone", controller: "User", AdditionalFields = nameof(Id)
        , ErrorMessage = "User exist with this Phone number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "User Name is required.")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Username can contain only alphabets.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 20 characters.")]
        [Remote(action: "CheckUsername", controller: "User", AdditionalFields = nameof(Id)
        , ErrorMessage = "Username is already taken.")]

        public string Username { get; set; }

        public string Rolename { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Country is required.")]

        public int CountryId { get; set; }

        public string Profileimg { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        public string? Address { get; set; }

        public string? Zipcode { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SelectListItem> Cities { get; set; }

        public List<SelectListItem> Roles { get; set; }

}
