using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.ViewModels;
using System.Security.Claims;

namespace SmartNovel.ViewComponents.NewFolder
{
    public class HotNovelsViewComponent : ViewComponent
    {
        private readonly SmartTruyenDbContext _context ;

        public HotNovelsViewComponent(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier); var crruentUser = await _context.Users

                .Include(u => u.Authors)
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.Uid == currentUserId);

            var blockCategoryIds = crruentUser?.Categories.Select(c => c.CategoryId).ToList() ?? new List<string>();
            var blockAuthorIds = crruentUser?.Authors.Select(c => c.Uid).ToList() ?? new List<string>();

            var hotNovels = _context.Novels
                .Include(u => u.Categories)
                .Include(u => u.Ratings)
                .Where(n => n.Status == "Public" )
                .Where(n => !blockAuthorIds.Contains(n.Uid))
                .Where(n => !n.Categories.Any(c => blockCategoryIds.Contains(c.CategoryId)))
                .OrderByDescending(u => u.ViewCount)
                .ThenBy(u => u.LikeCount)
                .Take(9);
            DateTime now = DateTime.Now;
            DateTime oneWeekAgo = now.AddDays(-7);
            DateTime oneMonthAgo = now.AddMonths(-1);

            var viewModel = new HotNovelsViewModel();

            viewModel.Weekly = await hotNovels
                .OrderByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneWeekAgo))
                .Take(9)
                .ToListAsync();

            viewModel.Monthly = await hotNovels
                .OrderByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneMonthAgo))
                .Take(9)
                .ToListAsync();

            return View(viewModel);
        }
    }
}
