using System.ComponentModel.DataAnnotations;

namespace pizzashop.repository.ViewModels;

public class PermissionViewModel
{
    public string Rolename { get; set; }
   public int PermissionId { get; set; }

    public string ModuleName { get; set; }

    public bool CanView { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }
    public bool? Canaddedit { get; internal set; }
}
