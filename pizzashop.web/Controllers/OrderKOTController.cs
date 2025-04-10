using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pizzashop.web.Controllers;

public class OrderKOTController : Controller
{
    public ActionResult KOT(){
        return View();
    }
}
