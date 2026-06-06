using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;

namespace SmartNovel.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly SmartTruyenDbContext _context;

        public HeaderViewComponent(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menus = await _context.MenuNavs
                .Where(x => x.RoleId == "5" &&
                            x.ParentId == null)
                .Include(x => x.InverseParent)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return View(menus);
        }
    }
}