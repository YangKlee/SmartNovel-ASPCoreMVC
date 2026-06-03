using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartNovel.ViewComponents.NewFolder
{
    public class RecommendedNovelsViewComponent : ViewComponent
    {
        private readonly SmartTruyenDbContext _context;
        public RecommendedNovelsViewComponent(SmartTruyenDbContext context) { _context = context; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _context.Users
                .Include(u => u.Categories)
                .Include(u => u.Authors)
                .FirstOrDefaultAsync(u => u.Uid == currentUserId);

            var blockCategoryIds = currentUser?.Categories.Select(c => c.CategoryId).ToList() ?? new List<string>();
            var blockAuthorIds = currentUser?.Authors.Select(c => c.Uid).ToList() ?? new List<string>();

            var followedAuthorIds = await _context.Users
            .Where(u => u.Uid == currentUserId)
            .SelectMany(u => u.UidsNavigation) 
            .Select(u => u.Uid)
            .ToListAsync();


            var historyCategoryIds = await _context.HistoryReaders
                .Where(u => u.Uid == currentUserId)
                .SelectMany(u => u.Novel.Categories)
                .Select(c => c.CategoryId)
                .Distinct()
                .ToListAsync();

            var followedAuthorCategoryIds = await _context.Novels
                .Where(n => followedAuthorIds.Contains(n.Uid))
                .SelectMany(n => n.Categories)
                .Select(c => c.CategoryId)
                .Distinct()
                .ToListAsync();

            var targetCategoryIds = historyCategoryIds.Union(followedAuthorCategoryIds).Distinct().ToList();

            var basedQuery = _context.Novels
                .Include(u => u.Categories)
                .Include(u => u.Chapters)
                .Where(u => u.Status == "Ongoing" || u.Status == "Completed")
                .Where(n => !blockAuthorIds.Contains(n.Uid))
                .Where(n => !n.Categories.Any(c => blockCategoryIds.Contains(c.CategoryId)))
                .Where(n => followedAuthorIds.Contains(n.Uid) || n.Categories.Any(c => targetCategoryIds.Contains(c.CategoryId)));

            DateTime now = DateTime.Now;
            DateTime oneDayAgo = now.AddDays(-1);
            DateTime oneWeekAgo = now.AddDays(-7);
            DateTime oneMonthAgo = now.AddMonths(-1);

            var dailyList = await basedQuery
                .OrderByDescending(n => followedAuthorIds.Contains(n.Uid)) 
                .ThenByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneDayAgo)) 
                .ThenByDescending(n => n.ViewCount) 
                .Take(9)
                .ToListAsync();

            var weeklyList = await basedQuery
                .OrderByDescending(n => followedAuthorIds.Contains(n.Uid))
                .ThenByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneWeekAgo))
                .ThenByDescending(n => n.ViewCount)
                .Take(9)
                .ToListAsync();

            var monthlyList = await basedQuery
                .OrderByDescending(n => followedAuthorIds.Contains(n.Uid))
                .ThenByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneMonthAgo))
                .ThenByDescending(n => n.ViewCount)
                .Take(9)
                .ToListAsync();

            var viewModel = new RecommendedViewModel { Daily = dailyList, Weekly = weeklyList, Monthly = monthlyList };
            return View(viewModel);
        }
    }
}