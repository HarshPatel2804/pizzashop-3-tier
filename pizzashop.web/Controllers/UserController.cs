using Microsoft.AspNetCore.Mvc;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;
using pizzashop.service.Implementations;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;
using System.Threading.Tasks;

namespace pizzashop.web.Controllers;


public class UserController : Controller
{
    private readonly IUsersLoginService _usersLoginService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ICountryService _countryService;
    private readonly IEmailService _EmailService;

    private readonly PizzaShopContext _context;


    public UserController(IUsersLoginService usersLoginService, IUserService userService, IRoleService roleService, ICountryService countryService, IEmailService EmailService, PizzaShopContext context)
    {
        _usersLoginService = usersLoginService;
        _userService = userService;
        _roleService = roleService;
        _countryService = countryService;
        _EmailService = EmailService;
        _context = context;
    }

    [CustomAuthorize("Users", "CanView")]
    public async Task<IActionResult> UserList(int page = 1, int pageSize = 5, string search = "", string sortColumn = "", string sortOrder = "")
    {
        var id = SessionUtils.GetUser(HttpContext);

        var (users, totalUsers, totalPages) = await _usersLoginService.GetPaginatedUsersAsync(page, pageSize, search, sortColumn, sortOrder,(int)id.Id);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
            ? PartialView("_ShowUserList", users)
            : View(users);
    }

    [CustomAuthorize("Users", "CanDelete")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUser(id);
        TempData["SuccessMessage"] = "User deleted successfully!";
        return RedirectToAction("UserList", "User");
    }

    [CustomAuthorize("Users", "CanAddEdit")]
    public async Task<IActionResult> AddUser()
    {

        var model = new UserViewModel();

        // First, get roles
        model.Roles = await _roleService.GetAllRoles();

        // Then, get countries
        model.Countries = await _countryService.GetAllCountry();

        return View(model);

    }

    [CustomAuthorize("Users", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddUser(UserViewModel model, IFormFile ProfileImage)
    {
        ModelState.Remove(nameof(model.Countries));
        ModelState.Remove(nameof(model.States));
        ModelState.Remove(nameof(model.Cities));
        ModelState.Remove(nameof(model.Roles));
        ModelState.Remove(nameof(model.Profileimg));
        ModelState.Remove(nameof(ProfileImage));
        if (!ModelState.IsValid) return View(model);
        await _userService.AddUser(model, ProfileImage);
        await _EmailService.SendEmailtoNewUserAsync(model.Email, model.FirstName, model.Password);
        TempData["SuccessMessage"] = "User added successfully!";
        return RedirectToAction("UserList", "User");
    }

    [CustomAuthorize("Users", "CanAddEdit")]
    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var model = await _userService.GetUserData(id);
        return View(model);
    }

    [CustomAuthorize("Users", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> EditUser(UserViewModel model, IFormFile ProfileImage)
    {
        Console.WriteLine("edit");
        ModelState.Remove(nameof(model.Countries));
        ModelState.Remove(nameof(model.States));
        ModelState.Remove(nameof(model.Cities));
        ModelState.Remove(nameof(model.Roles));
        ModelState.Remove(nameof(model.Email));
        ModelState.Remove(nameof(model.Password));
        ModelState.Remove(nameof(model.Profileimg));
        ModelState.Remove(nameof(ProfileImage));
        ModelState.Remove(nameof(model.Rolename));
        if (!ModelState.IsValid) return View(model);
        await _userService.UpdateUserData(model, ProfileImage);
        TempData["SuccessMessage"] = "User edited successfully!";
        return RedirectToAction("UserList", "User");
    }

    [HttpGet]
    public JsonResult CheckUsername(string Username, int? Id = null)
    {
        var isUsernameTaken = _usersLoginService.CheckUsername(Username, Id);

        return Json(!isUsernameTaken);
    }

    [HttpGet]
    public JsonResult CheckPhone(string Phone, int? Id = null)
    {
        var isPhoneTaken = _userService.CheckPhone(Phone, Id);

        return Json(!isPhoneTaken);
    }

    [HttpGet]
    public JsonResult CheckEmail(string Email, int? Id = null)
    {
        var isEmailTaken = _usersLoginService.CheckEmail(Email, Id);

        return Json(!isEmailTaken);
    }
}
