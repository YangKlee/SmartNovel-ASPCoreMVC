using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.ViewModels.AdminCategory;
using X.PagedList;
using X.PagedList.Extensions;

namespace SmartNovel.Controllers
{
    [Authorize(Roles = "1")]
    public class AdminCategoryController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public AdminCategoryController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string keyword, string status, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Name.Contains(keyword) || (c.Description != null && c.Description.Contains(keyword)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }

            var categories = query.ToPagedList(page, pageSize);

            ViewBag.Keyword = keyword;
            ViewBag.Status = status;

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CategoryViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại!";
                return View(model);
            }

            bool isExists = await _context.Categories.AnyAsync(c => c.Name == model.Name);
            if (isExists)
            {
                TempData["ErrorMessage"] = "Tên thể loại đã tồn tại!";
                return View(model);
            }

            var newCategory = new Category
            {
                CategoryId = Guid.NewGuid().ToString(),
                Name = model.Name,
                Description = model.Description,
                Slug = string.IsNullOrEmpty(model.Slug) ? Guid.NewGuid().ToString() : model.Slug,
                Status = model.Status
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã thêm thành công thể loại: {newCategory.Name}";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return NotFound();

            var model = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                Status = category.Status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu chỉnh sửa không hợp lệ!";
                return View(model);
            }

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == model.CategoryId);
            if (existingCategory == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thể loại cần chỉnh sửa!";
                return RedirectToAction("Index");
            }

            bool nameConflict = await _context.Categories.AnyAsync(c => c.Name == model.Name && c.CategoryId != model.CategoryId);
            if (nameConflict)
            {
                TempData["ErrorMessage"] = "Tên thể loại này đã được sử dụng!";
                return View(model);
            }

            existingCategory.Name = model.Name;
            existingCategory.Description = model.Description;
            if(!string.IsNullOrEmpty(model.Slug)) {
                existingCategory.Slug = model.Slug;
            }
            existingCategory.Status = model.Status;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật thông tin thể loại: {existingCategory.Name}";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await _context.Categories.Include(c => c.Novels).FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thể loại cần xóa!";
                return RedirectToAction("Index");
            }

            if (category.Novels != null && category.Novels.Any())
            {
                TempData["ErrorMessage"] = "Thể loại này đang chứa truyện, không thể xóa!";
                return RedirectToAction("Index");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa thành công thể loại: {category.Name}";
            return RedirectToAction("Index");
        }
    }
}
