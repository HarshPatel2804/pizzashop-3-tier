using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Permission
{
    public int Permissionid { get; set; }

    public int Roleid { get; set; }

    public int Moduleid { get; set; }

    public bool? Canview { get; set; }

    public bool? Canaddedit { get; set; }

    public bool? Candelete { get; set; }

    public virtual Permissionmodule Module { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
