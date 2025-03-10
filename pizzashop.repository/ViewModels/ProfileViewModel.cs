using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.ViewModels;

public class ProfileViewModel
{
   public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }
    
    public int CountryId { get; set; }

    public string Profileimg {get; set;}

    public int StateId { get; set; }

    public int CityId { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public List<SelectListItem> Countries {get; set;}

     public List<SelectListItem> States {get; set;}

      public List<SelectListItem> Cities {get; set;}

    public List<SelectListItem> Roles {get; set;}

}
