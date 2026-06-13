using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;
using System.Diagnostics;

namespace SmartNovel.Controllers
{
    public class HomeController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        public HomeController(SmartTruyenDbContext context) { _context = context; }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("debug-follow")]
        public async Task<IActionResult> DebugFollow(string uid = "7e149b58-77a2-4f47-ad13-9cca5924aa32")
        {
            var user = _context.Users.Where(u => u.Uid == uid).FirstOrDefault();
            var followerUs = _context.Users.Where(u => u.Uid == uid).SelectMany(u => u.FollowerUs).Select(u => u.Uid).ToList();
            var uidsNav = _context.Users.Where(u => u.Uid == uid).SelectMany(u => u.UidsNavigation).Select(u => u.Uid).ToList();
            
            return Json(new {
                User = user?.Username,
                FollowerUs = followerUs,
                UidsNavigation = uidsNav
            });
        }
    }
}
