using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModels;
using System.Security.Claims;

namespace SmartNovel.Controllers
{
    [Authorize]
    public class FollowingController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public FollowingController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.FollowerUs)
                .Include(x => x.Novels)
                    .ThenInclude(x => x.Categories)
                .Include(x => x.Novels)
                    .ThenInclude(x => x.UidNavigation)
                .FirstOrDefault(x => x.Uid == uid);

            if (user == null)
                return NotFound();

            var vm = new FollowingNovelVM
            {
                Novels = user.Novels
                    .OrderBy(x => x.Title)
                    .ToList(),
                author = user.FollowerUs.ToList()
            };

            return View("~/Views/Novel/Following.cshtml", vm);
        }

        [HttpPost]
        public async Task<IActionResult> UnfollowNovel(string novelId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(x => x.Novels)
                .FirstOrDefaultAsync(x => x.Uid == uid);

            if (user == null)
                return NotFound();

            var novel = user.Novels
                .FirstOrDefault(x => x.NovelId == novelId);

            if (novel != null)
            {
                user.Novels.Remove(novel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnfollowAuthor(string authorId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var author = await _context.Users.FirstOrDefaultAsync(u => u.Uid == authorId);
            var user = await _context.Users.Include(u => u.FollowerUs).FirstOrDefaultAsync(u => u.Uid == uid);

            if (author == null || user == null)
                return NotFound();

            user.FollowerUs.Remove(author);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
