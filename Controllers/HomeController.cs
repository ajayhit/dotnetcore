using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string useid,string pass)
        {
            CookieContainer cookieContainer = new CookieContainer();
            string deviceinfo = Guid.NewGuid().ToString();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };
            handler.CookieContainer = cookieContainer;
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("X-Device-info", deviceinfo);
            var req = new
            {
                //Email = "user@secureapi.com",
                //Password = "Pa$$w0rd." 
                Email = "aj@ajay.com",
                Password = "Ajay@123"
            };
            var req1 = JsonConvert.SerializeObject(req);
            var stringContent = new StringContent(req1, Encoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+


            var loginResponse =await  client.PostAsync("https://localhost:44347/api/User/token", stringContent);
            var chk = loginResponse.Content.ReadAsStringAsync().Result; 


            var authCookie = cookieContainer.GetCookies(new Uri("https://localhost:44347/")).Cast<Cookie>().Single(cookie => cookie.Name == "refreshToken");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(10),
            };
            Response.Cookies.Append("refreshToken", authCookie.Value, cookieOptions);
            Response.Cookies.Append("jwt", deviceinfo, cookieOptions);
            return RedirectToAction("Index", "Home", new { Area = "Users" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
