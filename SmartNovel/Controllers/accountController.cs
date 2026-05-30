using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using SmartNovel.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace SmartNovel.Controllers
{
    public class accountController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        private readonly MailServices _mailServices;
        private readonly IMemoryCache _cache;

        public accountController(SmartTruyenDbContext context, MailServices mailServices, IMemoryCache cache)
        {
            _mailServices = mailServices;
            _context = context;
            _cache = cache;
        }
        public async Task<IActionResult> Index()
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (uid == null)
            {
                return Redirect("auth/Login");
            }
            var model = await _context.Users.FirstOrDefaultAsync(x => x.Uid == uid);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> updateInfo()
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid == null)
            {
                return Redirect("auth/Login");
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Uid == uid);
            var model = new UserProfileViewModel();
            model.Phone = user.Phone;
            model.Birthday = user.Birthday;
            model.DisplayName = user.DisplayName;
            return View("ChangeInfo", model);
        }
        [HttpPost]
        public async Task<IActionResult> changePassword(ChangePassword req)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid == null)
            {
                return Redirect("auth/Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Uid == uid);

            var hashPassword = new PasswordHasher<object>();
            var resultCheckPassword = hashPassword.VerifyHashedPassword(null, user.Password, req.OldPassword);
            if(resultCheckPassword == PasswordVerificationResult.Failed)
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Mật khẩu cũ không đúng";
                return View("ChangePassword");
            }
            user.Password = hashPassword.HashPassword(null, req.NewPassword);
            try
            {
                await _context.SaveChangesAsync();
                ViewBag.Success = true;
                ViewBag.Msg = "Cập nhật mật khẩu thành công!";
                return View("ChangePassword");
            }
            catch
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Something went wrong bro:)";
                return View("ChangePassword");
            }

        }
        [HttpGet]
        public async Task<IActionResult> changePassword()
        {
            return View("ChangePassword");
        }
        [HttpPost]
        public async Task<IActionResult> updateInfo(UserProfileViewModel req)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid == null)
            {
                return Redirect("auth/Login");
            }
           
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Uid == uid);
            user.DisplayName = req.DisplayName;
            user.Birthday = req.Birthday;
            user.Phone = req.Phone;
            try
            {
                await _context.SaveChangesAsync();
                ViewBag.Success = true;
                ViewBag.Msg = "Thành công!";
                return View("ChangeInfo", req);
            }
            catch
            {
                var phoneExists = await _context.Users.AnyAsync(x =>
                    x.Phone == req.Phone&&
                    x.Uid != uid
                );
                if(phoneExists)
                {
                    ViewBag.Success = false;
                    ViewBag.Msg = "Số điện thoại đã tồn tại";
                }
            }


            return View("ChangeInfo", req);
        }
    }
}
