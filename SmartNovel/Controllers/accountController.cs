using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        private readonly FileStorageServices _fileServicesUpload;
        public accountController(SmartTruyenDbContext context, MailServices mailServices, IMemoryCache cache, FileStorageServices fileServicesUpload)
        {
            _mailServices = mailServices;
            _context = context;
            _cache = cache;
            _fileServicesUpload = fileServicesUpload;
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
            // lấy danh sách người mình theo dõi
            .Include(x => x.UidsNavigation)
            // lấy danh sách người theo dõi mình
            .Include(x => x.FollowerUs)
            .Include(x => x.NovelsNavigation)
            .FirstOrDefaultAsync(x => x.Uid == uid);
            if (model.RoleId == "3") //author
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
            if (model.RoleId == "4") //reader
            {
                ViewBag.FollowNovelCount =
                    model.Novels.Count;

                ViewBag.FollowAuthorCount =
                    model.UidsNavigation.Count;
            }
            
            ViewBag.FollowingAuthors = model.UidsNavigation.ToList();
            ViewBag.Followers = model.FollowerUs.ToList();

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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> changeProfileImage()
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid == null)
            {
                return Redirect("auth/Login");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Uid == uid);
            var model = new ChangeProfileViewModel
            {
                CurrentImage = user.AvartarUrl,

            };
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> changeProfileImage(ChangeProfileViewModel req)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string publicLink = "https://pub-20056e4912f440f08b3d40eea545f95f.r2.dev/smart-novel/user-image/";
            long maxFileSizeInBytes = 5 * 1024 * 1024;
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Uid == uid);
            if (req.NewImage != null)
            {
                var extension = Path.GetExtension(req.NewImage.FileName).ToLowerInvariant();
                if (req.NewImage.Length > maxFileSizeInBytes && req.NewImage.Length == 0)
                {
                    ViewBag.Success = false;
                    ViewBag.Msg = "File upload phải > 0b đến <= 5mb";
                    return View(new ChangeProfileViewModel { CurrentImage = req.CurrentImage });
                }


                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                {
                    ViewBag.Success = false;
                    ViewBag.Msg = "Loại file không hợp lệ";
                    return View(new ChangeProfileViewModel { CurrentImage = req.CurrentImage });
                }
                if (!string.IsNullOrEmpty(user.AvartarUrl))
                {
                    var fileOldName = user.AvartarUrl.Replace(publicLink, "");
                    await _fileServicesUpload.DeleteFile("smart-novel/user-image/", fileOldName);
                }
                string fileName = Guid.NewGuid().ToString() + "-" + req.NewImage.FileName;
                var res = await _fileServicesUpload.UploadFile("smart-novel/user-image/", fileName, req.NewImage);
                if(res)
                {
                    

                    user.AvartarUrl = publicLink + fileName;
                    await _context.SaveChangesAsync();
                    ViewBag.Success = true;
                    ViewBag.Msg = "Tải ảnh đại diện thành công~!";
                    
                    return View(new ChangeProfileViewModel { CurrentImage = user.AvartarUrl });

                }
                ViewBag.Success = false;
                ViewBag.Msg = "Không upload file được lên máy chủ";
                return View(new ChangeProfileViewModel { CurrentImage = req.CurrentImage });
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Msg = "FIle upload trống hoặc bị lỗi";
                return View(new ChangeProfileViewModel { CurrentImage = req.CurrentImage });
            }


        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> changeAuthor()
        {
            var uid = User.FindFirst(ClaimTypes.Role)?.Value;
            if (uid == "4")
                return View();
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> dochangeAuthor()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (role == "4")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Uid == uid);
                user.RoleId = "3";
                await _context.SaveChangesAsync();

                // Cập nhật lại session/cookie với quyền mới
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Uid),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.RoleId)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                TempData["Msg"] = "Chuyển tác giả thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
