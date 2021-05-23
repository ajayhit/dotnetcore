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
            //var client = new RestClient("https://localhost:44347/api/User/refresh-token");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.POST);
            //IRestResponse response = client.Execute(request);
            //dynamic resp = JsonConvert.DeserializeObject(response.Content);
            //var isAuthenticated = resp.isAuthenticated;
            //if(isAuthenticated==true)
            //{

            //}
            //Console.WriteLine(response.Content);

            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };
            handler.CookieContainer = cookieContainer;
            var client = new HttpClient(handler);


            var loginResponse = await client.PostAsync("https://localhost:44347/api/User/refresh-token", null);
            var chk = loginResponse.Content.ReadAsStringAsync().Result;


            var authCookie = cookieContainer.GetCookies(new Uri("https://localhost:44347/")).Cast<Cookie>().Single(cookie => cookie.Name == "refreshToken");
            cookieContainer.SetCookies(new Uri("https://localhost:44347/"), "refreshToken=" + authCookie.Value);
         //   return RedirectToAction("Index", "Home", new { Area = "Users" });

            return View();
        }
    }
}
