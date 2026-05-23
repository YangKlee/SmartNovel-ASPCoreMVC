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
            DateTime now = DateTime.Now;

            var adminRecommendNovels = await _context.RecommendNovels
                .Include(r => r.Novel)
                    .ThenInclude(n => n.Chapters) 
                .Include(r => r.Novel)
                    .ThenInclude(n => n.Categories) 
                .Select(r => r.Novel) 
                .Take(9) 
                .ToListAsync();

            return View(adminRecommendNovels);
        }
    }
}