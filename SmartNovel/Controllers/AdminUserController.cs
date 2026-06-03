using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.ViewModels;
using SmartNovel.ViewModels.AdminUser;

namespace SmartNovel.Controllers
{
    public class AdminUserController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public AdminUserController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string keyword, string role, string status, int page = 1)
        {
            int pageSize = 10; 

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.DisplayName.Contains(keyword) ||
                                         u.Username.Contains(keyword) ||
                                         u.Email.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.RoleId == role);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.Status == status);
            }

            int totalUsers = query.Count();
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }


        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateUserViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại!";
                return RedirectToAction("Index");
            }

            bool isExists = await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email);
            if (isExists)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc Email đã được sử dụng!";
                return RedirectToAction("Index");
            }

            var newUser = new User
            {
                Uid = Guid.NewGuid().ToString(),
                DisplayName = model.Displayname,
                Username = model.Username,
                Email = model.Email,
                Phone = model.PhoneNumber,
                CreatorPoint = model.CreatorPoint,
                Password = model.Password,
                RoleId = model.RoleID,
                Status = model.Status
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã thêm thành công tài khoản: {newUser.Username}";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Uid == id);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                Uid = user.Uid,
                Displayname = user.DisplayName,
                Username = user.Username,
                Email = user.Email,
                RoleID = user.RoleId,
                Status = user.Status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu chỉnh sửa không hợp lệ!";
                return RedirectToAction("Index");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Uid == model.Uid);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần chỉnh sửa!";
                return RedirectToAction("Index");
            }

            bool emailConflict = await _context.Users.AnyAsync(u => u.Email == model.Email && u.Uid != model.Uid);
            if (emailConflict)
            {
                TempData["ErrorMessage"] = "Email này đã được tài khoản khác sử dụng!";
                return RedirectToAction("Index");
            }

            existingUser.DisplayName = model.Displayname;
            existingUser.Email = model.Email;
            existingUser.RoleId = model.RoleID;
            existingUser.Status = model.Status;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                existingUser.Password = model.NewPassword;
            }

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật thông tin của: {existingUser.DisplayName}";
            return RedirectToAction("Index");
        }

    
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Uid == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần xóa!";
                return RedirectToAction("Index");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa thành công tài khoản: {user.Username}";
            return RedirectToAction("Index");
        }



    }

}