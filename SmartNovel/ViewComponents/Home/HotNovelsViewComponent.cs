using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.ViewModels;

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
            var CurrentUserId = "USER-0004";
            var crruentUser = await _context.Users
                .Include(u => u.Authors)
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.Uid == CurrentUserId);

            var blockCategoryIds = crruentUser?.Categories.Select(c => c.CategoryId).ToList() ?? new List<string>();
            var blockAuthorIds = crruentUser?.Authors.Select(c => c.Uid).ToList() ?? new List<string>();

            var hotNovels = _context.Novels
                .Include(u => u.Categories)
                .Where(n => n.Status == "Ongoing" || n.Status == "Completed")
                .Where(n => !blockAuthorIds.Contains(n.Uid))
                .Where(n => !n.Categories.Any(c => blockCategoryIds.Contains(c.CategoryId)))
                .OrderByDescending(u => u.ViewCount)
                .ThenBy(u => u.LikeCount)
                .Take(9);
            DateTime now = DateTime.Now;
            DateTime oneDayAgo = now.AddDays(-1);
            DateTime oneMonthAgo = now.AddMonths(-1);
            DateTime oneQuarterAgo = now.AddMonths(-3);

            var viewModel = new HotNovelsViewModel();

            viewModel.Daily = await hotNovels
                .OrderByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneDayAgo))
                .Take(9)
                .ToListAsync();

            viewModel.Monthly = await hotNovels
                .OrderByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneMonthAgo))
                .Take(9)
                .ToListAsync();

            viewModel.Quarterly = await hotNovels
                .OrderByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneQuarterAgo))
                .Take(9)
                .ToListAsync();

            viewModel.Sesonaly = await hotNovels
                .OrderByDescending(n => (n.ViewCount ?? 0) + (n.LikeCount ?? 0) * 10)
                .Take(9)
                .ToListAsync();

            
     
        return View(viewModel);
        }
    }
}
