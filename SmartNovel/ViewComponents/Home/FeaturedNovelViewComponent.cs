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
            var featuredNovel = await _context.Novels
                .Include(n => n.Categories)
                .Include(n => n.Chapters)
                .Include(n => n.UidNavigation) 
                .Where(n => n.Status != "Deleted")
                .OrderBy(x => Guid.NewGuid()) 
                .FirstOrDefaultAsync();

            return View(featuredNovel);
        }
    }
}