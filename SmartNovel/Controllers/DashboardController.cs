using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using SmartNovel.Models.ViewModels;
using System.Security.Claims;

namespace SmartNovel.Controllers
{
    public class DashboardController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        public DashboardController(SmartTruyenDbContext context) { _context = context; }
        public async Task<IActionResult> Index()
        {


            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allNovel = await _context.Novels.Where(n => n.Uid == uid).ToListAsync();
            // Lấy trực tiếp các chapter của tác giả này từ DB để tối ưu, không tải toàn bộ Chapters
            var myChapters = await _context.Chapters.Where(c => c.Novel.Uid == uid).ToListAsync();
            var user = await _context.Users.FirstOrDefaultAsync(n => n.Uid == uid);
            var allAuthor = await _context.Users.Where(u => u.RoleId == "3").OrderByDescending(u=> u.CreatorPoint).ToListAsync();
            var statsAuthor = new DashboardAuthorStatsViewModel
            {
                TotalNovels = allNovel?.Count(),
                DraftNovels = allNovel?.Count(n => n.Status != null && n.Status.ToLower() == "draft"),
                PublicNovels = allNovel?.Count(n => n.Status != null && n.Status.ToLower() == "public"),
                RemovedNovels = allNovel?.Count(n => n.Status != null && n.Status.ToLower() == "reject"),
                TotalChapters = myChapters.Count,
                PublicChapters = myChapters.Count(c => c.Status != null && c.Status.ToLower() == "public"),
                DraftChapters = myChapters.Count(c => c.Status != null && c.Status.ToLower() == "draft"),
                RemovedChapters = myChapters.Count(c => c.Status != null && (c.Status.ToLower() == "reject")),
                CreatorPoint = user?.CreatorPoint,
                Ranking = allAuthor.FindIndex(x => x.Uid == uid) + 1

            };
            var allComment = await _context.Comments
                .Include(c => c.Chapter).ThenInclude(ch => ch.Novel)
                .Include(c => c.UidNavigation)
                .Where(c=> c.Chapter.Novel.Uid == uid && c.Uid != uid)
                .OrderByDescending(c => c.TimeCommeny).Take(10).ToListAsync();
            var model = new DashboardViewModel
            {
                authorStats = statsAuthor,
                lastComment = allComment
            };
            return View(model);
        }
    }
}
