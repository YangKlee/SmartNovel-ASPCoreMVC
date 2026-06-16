
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using SmartNovel.Services;
using System.Security.Claims;
namespace SmartNovel.Controllers
{
    public class authController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        private readonly MailServices _mailServices;
        private readonly IMemoryCache _cache;

        public authController(SmartTruyenDbContext context, MailServices mailServices, IMemoryCache cache)
        {
            _mailServices = mailServices;
            _context = context;
            _cache = cache;
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
            var userTmp = await _context.Users.FirstOrDefaultAsync(e => e.Username == txtUsername || e.Email == txtUsername);
            if (userTmp == null)
            {
                ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng, vui lòng thử lại!";
                return View();
            }
            var result = hasher.VerifyHashedPassword(null, userTmp.Password, txtPassword);
            if(result == PasswordVerificationResult.Success)
            {
                if(userTmp.Status.ToLower() == "banned")
                {
                    return View("/Views/auth/Banned.cshtml");
                }
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
        public IActionResult LoginGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            if (claims == null) return RedirectToAction("Login");

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? email;

            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            var userTmp = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
            if (userTmp == null)
            {
               
                userTmp = new User
                {
                    Uid = Guid.NewGuid().ToString(),
                    Username = email, 
                    Email = email,
                    DisplayName = name,
                    Status = "Active",
                    RoleId = "4", // Default role
                    Password = new PasswordHasher<object>().HashPassword(null, Guid.NewGuid().ToString()) // random password
                };
                _context.Users.Add(userTmp);
                await _context.SaveChangesAsync();
            }

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userTmp.Uid),
                new Claim(ClaimTypes.Name, userTmp.Username),
                new Claim(ClaimTypes.Role, userTmp.RoleId)
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
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

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult LoginFacebook()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
            return Challenge(properties, Microsoft.AspNetCore.Authentication.Facebook.FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            if (claims == null) return RedirectToAction("Login");

            var nameIdentifier = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Người dùng Facebook";

            if (string.IsNullOrEmpty(email))
            {
                if (string.IsNullOrEmpty(nameIdentifier)) return RedirectToAction("Login");
                email = $"{nameIdentifier}@facebook.user";
            }

            var userTmp = await _context.Users.FirstOrDefaultAsync(e => e.Email == email || e.Username == email);
            
            if (userTmp == null)
            {
                userTmp = new User
                {
                    Uid = Guid.NewGuid().ToString(),
                    Username = email,
                    Email = email,
                    DisplayName = name,
                    Status = "Active",
                    RoleId = "4", 
                    Password = new PasswordHasher<object>().HashPassword(null, Guid.NewGuid().ToString())
                };
                _context.Users.Add(userTmp);
                await _context.SaveChangesAsync();
            }

            var userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userTmp.Uid),
        new Claim(ClaimTypes.Name, userTmp.Username),
        new Claim(ClaimTypes.Role, userTmp.RoleId)
    };

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
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

            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Regist()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> FoggotPassword()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailRecoveryPassword(ForgotPasswordViewModel obj)
        {
            var checkEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == obj.txtEmail);
            if (checkEmail == null)
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Mail không tồn tại trên hệ thống";

            }
            else
            {
                try
                {
                    var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    Guid token = Guid.NewGuid();
                    string scheme = Request.Scheme;
                    string host = Request.Host.Value;
                    string domain = $"{scheme}://{host}";

                    string linkRecovery = $"{domain}/Auth/RecoveryPassword?token={token.ToString()}";
                    string body = $"Liên kết khôi phục mật khẩu cho tài khoản{checkEmail.Username} là :" +
                        $" <a href='{linkRecovery}'>NHẤN VÀO ĐÂY ĐỂ KHÔI PHỤC</a>";
                    var result = await _mailServices.SendEmailAsync(obj.txtEmail, "Khôi phục mật khẩu", body);
                    if(result)
                    {
                        _cache.Set(token.ToString(), checkEmail.Email, cacheOptions);
                        ViewBag.Success = true;
                        ViewBag.Msg = "Mail gửi thành công, liên kết chỉ có hiệu lực trong 10 phút";
                    }
                    else
                    {
                        ViewBag.Success = false;
                        ViewBag.Msg = "Gửi không thành công";
                    }
                }
                catch
                {

                }
            }

            return View("FoggotPassword");
        }
        [HttpGet]
        public async Task<IActionResult> RecoveryPassword(string token)
        {
            var model = new ResetPasswordViewModel();
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Token không hợp lệ";

            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel obj)
        {
            var model = new ResetPasswordViewModel();
            bool isValid = _cache.TryGetValue(obj.Token, out string email);
            if (isValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    ViewBag.Success = false;
                    ViewBag.Msg = "Người dùng không tồn tại";
                }
                var passHash = new PasswordHasher<object>();

                user.Password = passHash.HashPassword(null, obj.txtNewPassword);
                await _context.SaveChangesAsync();
                ViewBag.Success = true;
                ViewBag.Msg = "Đổi mật khẩu thành công";
                _cache.Remove(obj.Token);
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Token không hợp lệ";
            }
            return View("RecoveryPassword", model);
        }
        [HttpPost]
        public async Task<IActionResult> Regist(RegisterViewModel newUser)
        {
            if(ModelState.IsValid)
            {
                var user = new User();
                Guid  uid = Guid.NewGuid();
                var passHash = new PasswordHasher<object>();
                user.Uid = uid.ToString();
                user.Username = newUser.txtUsername;
                user.Password = passHash.HashPassword(null, newUser.txtPassword);
                user.Status = "Active";
                user.RoleId = "4";
                user.Email = newUser.txtEmail;
                user.Phone = newUser.txtPhone;
                user.DisplayName = newUser.txtDisplayName;

                try
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    // báo thành công
                    ViewBag.Status = true;
                    ViewBag.Msg = "Đăng ký thành công!";
                    return RedirectToAction("Login", "Auth");
                }
                catch
                {
                    ViewBag.Status = false;
                    var checkUsername = await _context.Users.FirstOrDefaultAsync(x => x.Username == user.Username);
                    var checkEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
                    var checkPhone = await _context.Users.FirstOrDefaultAsync(x => x.Phone == user.Phone);
                    string errorContent = "";
                    if(checkUsername != null)
                    {
                        errorContent += "Username đã tồn tại!\n";
                    }
                    if(checkEmail != null)
                    {
                        errorContent += "Email đã tồn tại!\n";
                    }
                    if (checkPhone != null)
                    {
                        errorContent += "Phone đã tồn tại!\n";
                    }
                    ViewBag.Msg = errorContent;
                    return View();

                }
                
            }
            return View();

        }
    }
}
