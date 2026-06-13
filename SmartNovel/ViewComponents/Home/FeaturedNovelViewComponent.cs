using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartNovel.ViewComponents.NewFolder
{
    public class FeaturedNovelViewComponent : ViewComponent
    {
        private readonly SmartTruyenDbContext _context;
        public FeaturedNovelViewComponent(SmartTruyenDbContext context) { _context = context; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var uid = HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var blockedAuthorIds = new System.Collections.Generic.List<string>();

            // Lấy danh sách ID tác giả bị chặn nếu đã đăng nhập
            if (!string.IsNullOrEmpty(uid))
            {
                var currentUser = await _context.Users
                    .Include(u => u.Authors)
                    .FirstOrDefaultAsync(u => u.Uid == uid);

                if (currentUser != null)
                {
                    blockedAuthorIds = currentUser.Authors.Select(a => a.Uid).ToList();
                }
            }

            // Tạo truy vấn lấy truyện
            var query = _context.Novels
                .Include(n => n.Categories)
                .Include(n => n.Ratings)
                .Include(n => n.Chapters)
                .Include(n => n.UidNavigation)
                .Where(n => n.Status.ToLower() == "public");

            // Nếu có danh sách chặn, loại bỏ truyện của những tác giả đó
            if (blockedAuthorIds.Any())
            {
                query = query.Where(n => !blockedAuthorIds.Contains(n.Uid));
            }

            var featuredNovel = await query
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync();

            return View(featuredNovel);
        }
    }
}