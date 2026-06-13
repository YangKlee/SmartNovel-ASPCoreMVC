using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartNovel.Controllers
{
    public class CommentReaderController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public CommentReaderController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(CommentReaderViewModel req)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleId = User.FindFirstValue(ClaimTypes.Role);

            var query = _context.Comments
                .Include(c => c.Chapter).ThenInclude(ch => ch.Novel)
                .Include(c => c.UidNavigation)
                .AsQueryable();

            if (roleId == "3")
            {
                query = query.Where(c => c.Chapter.Novel.Uid == uid);
            }

            // Lấy danh sách truyện cho dropdown
            var novelsQuery = _context.Novels.AsQueryable();
            if (roleId == "3")
            {
                novelsQuery = novelsQuery.Where(n => n.Uid == uid);
            }
            ViewBag.Novels = await novelsQuery.Select(n => new { n.NovelId, n.Title }).ToListAsync();

            // Lọc theo Novel
            if (!string.IsNullOrEmpty(req.SelectedNovel))
            {
                query = query.Where(c => c.Chapter.NovelId == req.SelectedNovel);
                
                // Lấy danh sách chương cho Novel đã chọn
                ViewBag.Chapters = await _context.Chapters
                    .Where(ch => ch.NovelId == req.SelectedNovel)
                    .OrderBy(ch => ch.ChaperOrder)
                    .Select(ch => new { ch.ChapterId, ch.ChapterTitle })
                    .ToListAsync();
            }

            // Lọc theo Chapter
            if (!string.IsNullOrEmpty(req.SelectedChapter))
            {
                query = query.Where(c => c.ChapterId == req.SelectedChapter);
            }

            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(c => c.Content.Contains(req.Keyword) 
                                      || c.UidNavigation.DisplayName.Contains(req.Keyword)
                                      || c.UidNavigation.Username.Contains(req.Keyword));
            }

            req.TotalItem = await query.CountAsync();

            if (req.PageSize <= 0) req.PageSize = 5;
            if (req.CurrentPage <= 0) req.CurrentPage = 1;

            req.Comments = await query
                .OrderByDescending(c => c.TimeCommeny)
                .Skip((req.CurrentPage - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            return View(req);
        }
    }
}
