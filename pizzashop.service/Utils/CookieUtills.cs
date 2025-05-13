using pizzashop.repository.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Utils
{
    public static class CookieUtils
    {
        /// <summary>
        /// Save JWT Token to Cookies
        /// </summary>
        /// <param name="response"></param>
        /// <param name="token"></param>
        public static void SaveJWTToken(HttpResponse response, string token)
        {
            response.Cookies.Append("SuperSecretAuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(3)
            });
        }

        public static string? GetJWTToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("SuperSecretAuthToken", out string? token);
            return token;
        }

        /// <summary>
        /// Save User data to Cookies
        /// </summary>
        /// <param name="response"></param>
        /// <param name="user"></param>
        public static void SaveUserData(HttpResponse response, Userslogin userLogin, User user)
        {
             CookiesViewModel cookieUserData = new CookiesViewModel
            {
                Id = userLogin.Userid,
                Email = userLogin.Email,
                Username = userLogin.Username,
                ProfileImg = user.Profileimg,
                roleId = userLogin.Roleid
            };

            Console.WriteLine(cookieUserData.ProfileImg);

            string userData = JsonSerializer.Serialize(cookieUserData);

            // Store user details in a cookie for 3 days
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(3),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            };
            response.Cookies.Append("UserData", userData, cookieOptions);
        }

        public static void ClearCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("SuperSecretAuthToken");
            httpContext.Response.Cookies.Delete("UserData");
        }
    }
}