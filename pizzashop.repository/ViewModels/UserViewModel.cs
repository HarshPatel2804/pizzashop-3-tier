using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.ViewModels;

public enum statustype
{
    Active = 1,
    Inactive = 2
}

public class UserViewModel
{
   public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public string Username { get; set; }

    public string Password {get; set; }

    public int Role {get; set;}

    public string Email { get; set; }
    
    public int CountryId { get; set; }

    public int StateId { get; set; }

    public int CityId { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public statustype status { get; set; }

    public List<SelectListItem> Countries {get; set;}

     public List<SelectListItem> States {get; set;}

      public List<SelectListItem> Cities {get; set;}

    public List<SelectListItem> Roles {get; set;}

}
