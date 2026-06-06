using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;
using System.Diagnostics;

namespace SmartNovel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
