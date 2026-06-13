using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartNovel.ViewComponents.NewFolder
{
    public class SidebarAdminRecommendViewComponent : ViewComponent
    {
        private readonly SmartTruyenDbContext _context;
        public SidebarAdminRecommendViewComponent(SmartTruyenDbContext context) { _context = context; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var uid = HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var blockedAuthorIds = new System.Collections.Generic.List<string>();

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

            DateTime now = DateTime.Now;

            var query = _context.RecommendNovels
                .Include(r => r.Novel)
                    .ThenInclude(n => n.Chapters) 
                .Include(r => r.Novel)
                    .ThenInclude(n => n.Categories) 
                .AsQueryable();

            if (blockedAuthorIds.Any())
            {
                query = query.Where(r => !blockedAuthorIds.Contains(r.Novel.Uid));
            }

            var adminRecommendNovels = await query
                .Select(r => r.Novel) 
                .Take(9) 
                .ToListAsync();

            return View(adminRecommendNovels);
        }
    }
}