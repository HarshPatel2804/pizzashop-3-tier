using System.Text.Json;
using Microsoft.AspNetCore.Http;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Utils;

public static class SessionUtils
{
    public static void SetUser(HttpContext httpContext, Userslogin userLogin, User user)
    {
        if(user != null)
        {
            CookiesViewModel cookieUserData = new CookiesViewModel
            {
                Id = userLogin.Userid,
                Email = userLogin.Email,
                Username = userLogin.Username,
                ProfileImg = user.Profileimg
            };


            string userData = JsonSerializer.Serialize(cookieUserData);
            httpContext.Session.SetString("UserData", userData);

            // httpContext.Session.SetString("UserId", user.Userid.ToString());
        }
    }

    public static CookiesViewModel? GetUser(HttpContext httpContext)
    {
        string? userData = httpContext.Session.GetString("UserData");

        if(string.IsNullOrEmpty(userData))
        {
            httpContext.Request.Cookies.TryGetValue("UserData", out userData);
        }

        return string.IsNullOrEmpty(userData) ? null : JsonSerializer.Deserialize<CookiesViewModel>(userData);
    }

    public static void ClearSession (HttpContext httpContext) => httpContext.Session.Clear();

    internal static void SetUser(object httpContext, Userslogin usersLogin)
    {
        throw new NotImplementedException();
    }
}
