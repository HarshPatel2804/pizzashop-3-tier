using Microsoft.AspNetCore.Mvc;
using pizzashop.repository.ViewModels;
using pizzashop.service.Implementations;
using pizzashop.service.Interfaces;
using System.Threading.Tasks;

namespace pizzashop.web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUsersLoginService _usersLoginService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ICountryService _countryService;

        public UserController(IUsersLoginService usersLoginService, IUserService userService, IRoleService roleService, ICountryService countryService)
        {
            _usersLoginService = usersLoginService;
            _userService = userService;
            _roleService = roleService;
            _countryService = countryService;
        }

        public async Task<IActionResult> UserList(int page = 1, int pageSize = 5, string search = "")
        {
            var (users, totalUsers, totalPages) = await _usersLoginService.GetPaginatedUsersAsync(page, pageSize, search);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalPages = totalPages;
            // Console.WriteLine(totalUsers);
            // if(totalUsers == 0){
            //     TempData["ErrorMessage"] = "No User";
            // }

            return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                ? PartialView("_ShowUserList", users)
                : View(users);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUser(id);
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction("UserList", "User");
        }

        public async Task<IActionResult> AddUser()
        {

            var model = new UserViewModel
            {
                Roles = await _roleService.GetAllRoles(),
                Countries = await _countryService.GetAllCountry()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            await _userService.AddUser(model);
            return RedirectToAction("UserList", "User");
        }
    }
}
