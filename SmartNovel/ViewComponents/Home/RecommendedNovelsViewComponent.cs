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

        public RecommendedNovelsViewComponent(SmartTruyenDbContext context) 
        { 
            _context = context; 
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var viewModel = new RecommendedViewModel 
            { 
                Daily = new List<Novel>(), 
                Weekly = new List<Novel>(), 
                Monthly = new List<Novel>() 
            };

            if (string.IsNullOrEmpty(currentUserId))
                return View(viewModel);

            var userPreferences = await _context.Users
                .AsNoTracking()
                .Where(u => u.Uid == currentUserId)
                .Select(u => new
                {
                    BlockedCategories = u.Categories.Select(c => c.CategoryId).ToList(),
                    BlockedAuthors = u.Authors.Select(a => a.Uid).ToList(),
                    FollowedAuthors = u.UidsNavigation.Select(f => f.Uid).ToList(),
                    HistoryCategories = u.HistoryReaders.SelectMany(h => h.Novel.Categories).Select(c => c.CategoryId).Distinct().ToList()
                })
                .FirstOrDefaultAsync();

            if (userPreferences == null)
                return View(viewModel);

            var followedAuthorIds = userPreferences.FollowedAuthors;

            var followedAuthorCategoryIds = await _context.Novels
                .AsNoTracking()
                .Where(n => followedAuthorIds.Contains(n.Uid))
                .SelectMany(n => n.Categories)
                .Select(c => c.CategoryId)
                .Distinct()
                .ToListAsync();

            var targetCategoryIds = userPreferences.HistoryCategories.Union(followedAuthorCategoryIds).Distinct().ToList();

            if (!followedAuthorIds.Any() && !targetCategoryIds.Any())
                return View(viewModel);

            var query = _context.Novels
                .AsNoTracking()
                .Include(u => u.Categories)
                .Include(u => u.Chapters)
                .Include(u => u.Ratings)
                .Where(u => u.Status == "Public" || u.Status == "Completed");

            if (userPreferences.BlockedAuthors.Any())
                query = query.Where(n => !userPreferences.BlockedAuthors.Contains(n.Uid));

            if (userPreferences.BlockedCategories.Any())
                query = query.Where(n => !n.Categories.Any(c => userPreferences.BlockedCategories.Contains(c.CategoryId)));

            query = query.Where(n => followedAuthorIds.Contains(n.Uid) || n.Categories.Any(c => targetCategoryIds.Contains(c.CategoryId)));

            var now = DateTime.Now;
            var oneDayAgo = now.AddDays(-1);
            var oneWeekAgo = now.AddDays(-7);
            var oneMonthAgo = now.AddMonths(-1);

            viewModel.Daily = await query
                .OrderByDescending(n => followedAuthorIds.Contains(n.Uid))
                .ThenByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneDayAgo))
                .ThenByDescending(n => n.ViewCount)
                .Take(9)
                .ToListAsync();

            viewModel.Weekly = await query
                .OrderByDescending(n => followedAuthorIds.Contains(n.Uid))
                .ThenByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneWeekAgo))
                .ThenByDescending(n => n.ViewCount)
                .Take(9)
                .ToListAsync();

            viewModel.Monthly = await query
                .OrderByDescending(n => followedAuthorIds.Contains(n.Uid))
                .ThenByDescending(n => n.HistoryReaders.Count(h => h.TimeReader >= oneMonthAgo))
                .ThenByDescending(n => n.ViewCount)
                .Take(9)
                .ToListAsync();
            
            return View(viewModel);
        }
    }
}