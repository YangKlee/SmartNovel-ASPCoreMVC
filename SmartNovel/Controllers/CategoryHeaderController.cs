using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.ViewModels;

namespace SmartNovel.Controllers
{
    public class CategoryHeaderController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public CategoryHeaderController(
            SmartTruyenDbContext context)
        {
            _context = context;
        }

        [Route("the-loai/{slug}")]
        public async Task<IActionResult> Index(
            string slug,
            int currentPage = 1,
            int pageSize = 12)
        {
            var category = await _context.Categories
                .Include(x => x.Novels)
                .FirstOrDefaultAsync(x => x.Slug == slug);

            if (category == null)
            {
                return NotFound();
            }

            var totalItems = category.Novels.Count;

            var novels = category.Novels
                .OrderByDescending(x => x.CreateTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new CategoryHeaderVM
            {
                Slug = slug,
                CategoryName = category.Name,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(
                    (double)totalItems / pageSize),
                Novels = novels
            };

            return View(vm);
        }
    }
}