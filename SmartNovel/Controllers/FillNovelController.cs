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
            string? authorId,
            int? minRating,
            List<string>? selectedCategories,
            int page = 1)
        {
            const int pageSize = 12;

            // Kiểm tra xem người dùng có đang thực hiện tìm kiếm không?
            // Chỉ cần 1 trong các điều kiện này tồn tại, ta sẽ thực hiện query
            bool isSearching = !string.IsNullOrWhiteSpace(keyword) ||
                               !string.IsNullOrWhiteSpace(authorId) ||
                               minRating.HasValue ||
                               (selectedCategories != null && selectedCategories.Any());

            var novels = new List<Novel>();
            int totalItems = 0;

            // Chỉ truy vấn database nếu người dùng có yêu cầu tìm kiếm
            if (isSearching)
            {
                var query = _context.Novels
                    .Include(n => n.Categories)
                    .Include(n => n.UidNavigation)
                    .Include(n => n.Ratings)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    string k = keyword.Trim().ToLower();
                    query = query.Where(x => x.Title.ToLower().Contains(k));
                }

                if (!string.IsNullOrWhiteSpace(authorId))
                {
                    query = query.Where(x => x.Uid == authorId);
                }

                if (minRating.HasValue)
                {
                    query = query.Where(x => x.Ratings.Any() &&
                                       x.Ratings.Average(r => r.RatingPoint) >= minRating.Value);
                }

                if (selectedCategories != null && selectedCategories.Any())
                {
                    query = query.Where(x => x.Categories.Any(c => selectedCategories.Contains(c.CategoryId)));
                }

                // Sắp xếp
                query = query.OrderByDescending(x => x.UpdateTime);

                // Đếm và phân trang
                totalItems = await query.CountAsync();
                novels = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            // Đóng gói ViewModel
            var vm = new NovelSearchViewModel
            {
                Keyword = keyword,
                AuthorId = authorId,
                CurrentPage = page,
                MinRating = minRating,
                IsSearching = isSearching, // Truyền trạng thái này xuống View để ẩn/hiện danh sách
                TotalPages = isSearching ? (int)Math.Ceiling(totalItems / (double)pageSize) : 0,
                SelectedCategories = selectedCategories ?? new(),

                // Luôn lấy dữ liệu cho dropdown (Thể loại, Tác giả) để người dùng chọn
                Categories = await _context.Categories.OrderBy(x => x.Name).ToListAsync(),
                Authors = await _context.Users.Where(x => x.RoleId == "3").ToListAsync(),

                Novels = novels
            };

            return View(vm);
        }
    }
}