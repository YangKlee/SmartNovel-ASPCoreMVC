using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmartNovel.ViewComponents.NewFolder
{
    public class SidebarNewUpdateViewComponent : ViewComponent
    {
        private readonly SmartTruyenDbContext db;

        public SidebarNewUpdateViewComponent(SmartTruyenDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
        {
            //  User.Claims.FirstOrDefault(c => c.Type == "Uid")?.Value
            string currentUserId = "USER-0004";

            var currentUser = await db.Users
                .Include(u => u.Authors)
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.Uid ==currentUserId);

            var blockedCategoryIds = currentUser?.Categories.Select(c => c.CategoryId).ToList() ?? new List<string>();
            var blockedAuthorIds = currentUser?.Authors.Select(a => a.Uid).ToList() ?? new List<string>();

            var updateNewNovel = await db.Novels
                .Include( u => u.Chapters)
                .Where (u => u.Status =="Ongoing" || u.Status == "Completed")
                .Where(u => !u.Categories.Any(c =>blockedCategoryIds.Contains(c.CategoryId)))
                .Where(u => !blockedAuthorIds.Contains(u.Uid))
                .OrderByDescending(u => u.Chapters.Max(c => c.CreateTime))  
                .Take(9)
                .ToListAsync();
            return View(viewName, updateNewNovel);
        }
    }
}