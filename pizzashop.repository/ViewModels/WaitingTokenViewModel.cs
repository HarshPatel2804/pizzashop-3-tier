using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.ViewModels;

public partial class WaitingtokenViewModel
{
    [Required(ErrorMessage = "Number of people is required")]
    [Range(1, 100, ErrorMessage = "Number of people must be between 1 and 100")]
    public int Noofpeople { get; set; }

    [Required(ErrorMessage = "Section is required.")]
    public int Sectionid { get; set; }

    public bool? Isassigned { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[0-9]{10}$",
                ErrorMessage = "Phone number must be of 10 digits.")]
    public string Phoneno { get; set; } = null!;

    [Required(ErrorMessage = "Customer Name is required.")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Customer name can contain only alphabets.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 50 characters.")]
    public string Customername { get; set; } = null!;

    public List<SelectListItem> Sections { get; set; }

    public int Waitingtokenid { get; set; }

}
