using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pizzashop.web.Controllers;

public class OrderMenuController : Controller
{
    public ActionResult Menu(){
        return View();
    }
}
