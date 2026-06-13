using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;

namespace SmartNovel.Controllers
{
    public class AdminController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public AdminController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ListAuthors(AuthorListModel req)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == "3"); // Kiểm tra kiểu dữ liệu RoleId

            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(u =>
                    u.Username.Contains(req.Keyword) ||
                    u.DisplayName.Contains(req.Keyword));
            }

            req.TotalItems = await query.CountAsync();

            req.Authors = await query
                .OrderBy(u => u.Username)
                .Skip((req.CurrentPage - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            return View(
                "~/Views/AuthorListView/AuthorListView.cshtml",
                req
);
        }
    }
}