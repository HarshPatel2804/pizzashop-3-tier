using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pizzashop.web.Controllers;

public class OrderTableController : Controller
{
    public ActionResult Table(){
        return View();
    }
}
