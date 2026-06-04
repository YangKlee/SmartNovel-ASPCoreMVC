using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SmartNovel.Controllers
{
    public class NovelManagermentController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        public NovelManagermentController(SmartTruyenDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult>  Index([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string type = "all", [FromQuery] string keyword = "")
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = _context.Novels.AsNoTracking().Where(n => n.Uid == uid);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(n => n.Title.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(type) && type.ToLower() != "all")
            {
                query = query.Where(n => n.Status == type);
            }
            var totalCount = await query.CountAsync();
            var items = await query.Select(n => new NovelManagermentViewModel
            {
                NovelId = n.NovelId,
                Title = n.Title,
                Slug = n.Slug,
                Description = n.Description,
                AgeRating = n.AgeRating,
                ImageNovelUrl = n.ImageNovelUrl,
                ImageBanerNovelUrl = n.ImageBanerNovelUrl,
                Status = n.Status,
                ViewCount = n.ViewCount,
                LikeCount = n.LikeCount,
                CreateTime = n.CreateTime,
                UpdateTime = n.UpdateTime,
                categories = n.Categories,
                countChapter = n.Chapters.Count(),
                countChapterPublic = n.Chapters.Count(c => c.Status == "Public"),
                countChapterDraft = n.Chapters.Count(c => c.Status == "Draft"),
                countChapterRemove = n.Chapters.Count(c => c.Status == "Cancel"),
                novelRating = n.Ratings
                        .Select(r => (double?)r.RatingPoint)
                        .Average() ?? 0
            }).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var models = new ResultPartition<NovelManagermentViewModel>
            {
                data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount

            };

            return View(models);
        }
    }
}
