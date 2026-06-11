using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using SmartNovel.Models.ViewModels;
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
            var model = await _context.Users
            .Include(x => x.Novels)
            .Include(x => x.UidsNavigation)
            .Include(x => x.FollowerUs)
            .Include(x => x.NovelsNavigation)
            .FirstOrDefaultAsync(x => x.Uid == uid);
            if (model.RoleId == "3")
            {
                var publicNovels = model.NovelsNavigation
                    .Where(x => x.Status != null && x.Status.ToLower() == "public")
                    .OrderByDescending(x => x.ViewCount)
                    .ToList();
                ViewBag.PublicNovels = publicNovels;

                ViewBag.FollowerCount = model.FollowerUs.Count;

                ViewBag.TotalNovelCount = publicNovels.Count;

                ViewBag.TotalViewCount = publicNovels.Sum(x => x.ViewCount ?? 0);

                ViewBag.FollowNovelCount = model.Novels.Count;

                ViewBag.FollowAuthorCount = model.UidsNavigation.Count;
            }
            if (model.RoleId == "4")
            {
                ViewBag.FollowNovelCount =
                    model.Novels.Count;

                ViewBag.FollowAuthorCount =
                    model.UidsNavigation.Count;
            }
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

        //Profile
        [Route("profile/{uid}")]
        public IActionResult Profile(string uid)
        {
            var user = _context.Users
                .Include(x => x.Role)
                .Include(x => x.Novels)
                .Include(x => x.NovelsNavigation)
                .Include(x => x.FollowerUs)
                .Include(x => x.UidsNavigation)
                .FirstOrDefault(x => x.Uid == uid);

            if (user == null)
                return NotFound();

            var currentUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isFollowing = false;
            bool isBlocked = false;

            if (!string.IsNullOrEmpty(currentUid))
            {
                var currentUser = _context.Users
                    .Include(x => x.UidsNavigation)
                    .Include(x => x.Authors)
                    .FirstOrDefault(x => x.Uid == currentUid);

                if (currentUser != null)
                {
                    isFollowing = currentUser.UidsNavigation
                        .Any(x => x.Uid == uid);

                    isBlocked = currentUser.Authors
                        .Any(x => x.Uid == uid);
                }
            }

            var vm = new ProfileVM
            {
                User = user,
                IsFollowing = isFollowing,
                IsAuthorBlocked = isBlocked,

                PublicNovels = _context.Novels
                    .Where(x => x.Uid == uid
                        && x.Status != null
                        && x.Status.ToLower() == "public")
                    .ToList(),

                FollowingAuthors = user.UidsNavigation.ToList(),

                Followers = user.FollowerUs.ToList()
            };

            ViewBag.PublicNovels = vm.PublicNovels;
            ViewBag.FollowingAuthors = vm.FollowingAuthors;
            ViewBag.Followers = vm.Followers;

            ViewBag.FollowerCount = vm.Followers.Count;
            ViewBag.FollowAuthorCount = vm.FollowingAuthors.Count;
            ViewBag.TotalNovelCount = vm.PublicNovels.Count;
            ViewBag.TotalViewCount = vm.PublicNovels.Sum(x => x.ViewCount ?? 0);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FollowAuthor(string authorId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.UidsNavigation)
                .FirstOrDefault(x => x.Uid == uid);

            var author = _context.Users
                .FirstOrDefault(x => x.Uid == authorId);

            if (user == null || author == null)
                return NotFound();

            // không follow chính mình
            if (uid == authorId)
                return Redirect(Request.Headers["Referer"].ToString());

            if (!user.UidsNavigation.Any(x => x.Uid == authorId))
            {
                user.UidsNavigation.Add(author);
                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [Authorize]
        [HttpPost]
        public IActionResult UnFollowAuthor(string authorId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.UidsNavigation)
                .FirstOrDefault(x => x.Uid == uid);

            if (user == null)
                return NotFound();

            var author = user.UidsNavigation
                .FirstOrDefault(x => x.Uid == authorId);

            if (author != null)
            {
                user.UidsNavigation.Remove(author);
                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
