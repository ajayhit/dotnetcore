using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Areas.Users.Controllers
{
    [Area("Users")]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var jwt = Request.Cookies["jwt"];
            refreshToken = WebUtility.UrlDecode(refreshToken);
            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };
            handler.CookieContainer = cookieContainer;
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("X-Device-info", jwt);
            var req = new
            {
                Token = refreshToken
            };
            var req1 = JsonConvert.SerializeObject(req);
            var stringContent = new StringContent(req1, Encoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+


            var loginResponse = await client.PostAsync("https://localhost:44347/api/User/refresh-token", stringContent);
            var chk = loginResponse.Content.ReadAsStringAsync().Result;


            var authCookie = cookieContainer.GetCookies(new Uri("https://localhost:44347/")).Cast<Cookie>().Single(cookie => cookie.Name == "refreshToken");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(10),
            };
            Response.Cookies.Append("refreshToken", authCookie.Value, cookieOptions);

            return View();
        }
    }
}
