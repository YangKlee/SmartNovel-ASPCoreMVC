using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using System.Security.Claims;
namespace SmartNovel.Controllers
{
    public class authController : Controller
    {
        SmartTruyenDbContext _context;
        
        public authController(SmartTruyenDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return NotFound();
        }
        [HttpGet]
        public IActionResult Login()
        {
            // show form login chộ này
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string txtUsername, string txtPassword)
        {
            if(string.IsNullOrEmpty(txtUsername) || string.IsNullOrEmpty(txtPassword))
            {
                ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng, vui lòng thử lại!";
                return View();
            }
            var hasher = new PasswordHasher<User>();
            var userTmp = await _context.Users.FirstOrDefaultAsync(e => e.Username == txtUsername);
            if (userTmp == null)
            {
                ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng, vui lòng thử lại!";
                return View();
            }
            var result = hasher.VerifyHashedPassword(userTmp, userTmp.Password, txtPassword);
            if(result == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userTmp.Uid),
                    new Claim(ClaimTypes.Name, userTmp.Username),
                    new Claim(ClaimTypes.Role, userTmp.RoleId)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // nhớ login hay k
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1) // 1 ngày
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng, vui lòng thử lại!";
                return View();
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
