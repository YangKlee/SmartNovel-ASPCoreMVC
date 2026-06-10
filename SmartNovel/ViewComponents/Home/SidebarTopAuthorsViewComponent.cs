using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmartNovel.ViewComponents.NewFolder
{
    public class SidebarTopAuthorsViewComponent : ViewComponent
    {
        private readonly SmartTruyenDbContext db;

        public SidebarTopAuthorsViewComponent(SmartTruyenDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var topAuthor =await db.Users
            .Where(u => u.RoleId == "3" && u.Status == "Active")
            .OrderByDescending (u => u.CreatorPoint)
            .Take(7)
            .ToListAsync();

        return View(topAuthor);
        }
    }
}