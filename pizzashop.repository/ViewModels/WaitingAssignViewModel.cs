using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

    public class WaitingAssignViewModel
    {
        public int WaitingTokenId { get; set; }

        [Required(ErrorMessage = "Section is required.")]
        public int SectionId { get; set; }
        public List<int>? SelectedTableIds { get; set; } 
        public List<SelectListItem>? SectionList { get; set; }
        public List<SelectListItem>? TableList { get; set; }
    }
