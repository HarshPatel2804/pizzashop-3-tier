using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Attributes;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;
public class OrderKOTController:Controller{

    private readonly IKOTService _KotService;
    
    public OrderKOTController(IKOTService kOTService){
        _KotService = kOTService;
    }
    public async Task<IActionResult> KOT(){
        KOTViewModel kOTViewModel = await _KotService.GetKOTViewModel();
        return View(kOTViewModel);
    }

}