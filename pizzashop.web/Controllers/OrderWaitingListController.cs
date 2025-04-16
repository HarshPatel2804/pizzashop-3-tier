using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pizzashop.web.Controllers;

public class OrderWaitingListController : Controller
{
    public ActionResult WaitingList(){
        return View();
    }

    
}
