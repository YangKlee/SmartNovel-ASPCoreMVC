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
        public async Task<IActionResult>  Index(NovelManagermentViewModel req)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = _context.Novels.AsNoTracking().Where(n => n.Uid == uid);
            if (!string.IsNullOrWhiteSpace(req.Keyword))
            {
                query = query.Where(n => n.Title.Contains(req.Keyword));
            }
            if (!string.IsNullOrEmpty(req.SelectedStatus) && req.SelectedStatus.ToLower() != "all")
            {
                query = query.Where(n => n.Status == req.SelectedStatus);
            }
            var totalCount = await query.CountAsync();
            var items = await query.Select(n => new NovelViewModel
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
            }).Skip((req.CurrentPage - 1) * req.PageSize).Take(req.PageSize).ToListAsync();

            var models = new NovelManagermentViewModel
            {

                novels = items,
                PageSize = req.PageSize,
                TotalItem = totalCount,
                CurrentPage = req.CurrentPage

            };

            return View(models);
        }
    }
}
