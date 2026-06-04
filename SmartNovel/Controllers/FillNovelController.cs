using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.ViewModels;

namespace SmartNovel.Controllers
{
    public class FillNovelController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public FillNovelController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Search(
            string? keyword,
            string? status,
            string? authorId,
            string? sortBy,
            int? minRating,
            List<string>? selectedCategories,
            int page = 1)
        {
            const int pageSize = 12;

            var query = _context.Novels
                .Include(n => n.Categories)
                .Include(n => n.UidNavigation)
                .Include(n => n.Ratings)
                .AsQueryable();
            bool isSearching =
            !string.IsNullOrWhiteSpace(keyword) ||
            !string.IsNullOrWhiteSpace(status) ||
            !string.IsNullOrWhiteSpace(authorId) ||
            !string.IsNullOrWhiteSpace(sortBy) ||
            minRating.HasValue ||
            (selectedCategories != null && selectedCategories.Any());

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.Title.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x =>
                    x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(authorId))
            {
                query = query.Where(x =>
                    x.Uid == authorId);
            }
            if (minRating.HasValue)
            {
                query = query.Where(x =>
                    x.Ratings.Any() &&
                    x.Ratings.Average(r => r.RatingPoint) >= minRating.Value);
            }

            if (selectedCategories != null &&
                selectedCategories.Any())
            {
                query = query.Where(x =>
                    x.Categories.Any(c =>
                        selectedCategories.Contains(c.CategoryId)));
            }

            query = sortBy switch
            {
                "view" => query.OrderByDescending(x => x.ViewCount),
                "like" => query.OrderByDescending(x => x.LikeCount),
                "name" => query.OrderBy(x => x.Title),
                _ => query.OrderByDescending(x => x.UpdateTime)
            };

            int totalItems = 0;

            List<Novel> novels = new();

            if (isSearching)
            {
                totalItems = await query.CountAsync();

                novels = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            var vm = new NovelSearchViewModel
            {
                Keyword = keyword,
                Status = status,
                AuthorId = authorId,
                SortBy = sortBy,
                CurrentPage = page,
                IsSearching = isSearching,
                MinRating = minRating,
                TotalPages =
                    (int)Math.Ceiling(totalItems / (double)pageSize),

                SelectedCategories =
                    selectedCategories ?? new(),

                Categories =
                    await _context.Categories
                    .OrderBy(x => x.Name)
                    .ToListAsync(),

                Authors =
                    await _context.Users
                    .Where(x => x.RoleId == "3")
                    .ToListAsync(),

                Novels = novels
            };

            return View(vm);
        }
    }
}