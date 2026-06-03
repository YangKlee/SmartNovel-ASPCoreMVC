using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModels;

namespace SmartNovel.Controllers
{
    public class NovelController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public NovelController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        [Route("truyen/{slug}")]
        public IActionResult Detail(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            var novel = _context.Novels
                .Include(x => x.UidNavigation)
                .Include(x => x.Categories)
                .Include(x => x.Uids)
                .FirstOrDefault(x => x.Slug == slug);

            if (novel == null)
                return NotFound();

            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isFollowing = false;

            if (!string.IsNullOrEmpty(uid))
            {
                isFollowing = novel.Uids.Any(x => x.Uid == uid);
            }

            var vm = new NovelDetailVM
            {
                Novel = novel,
                Author = novel.UidNavigation,

                Categories = novel.Categories
                    .Select(x => x.Name)
                    .ToList(),

                Chapters = _context.Chapters
                    .Where(x => x.NovelId == novel.NovelId)
                    .OrderBy(x => x.ChaperOrder)
                    .ToList(),

                IsFollowing = isFollowing,

                FollowCount = novel.Uids.Count
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Follow(string novelId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.Novels)
                .FirstOrDefault(x => x.Uid == uid);

            var novel = _context.Novels
                .FirstOrDefault(x => x.NovelId == novelId);

            if (user == null || novel == null)
                return NotFound();

            if (!user.Novels.Any(x => x.NovelId == novelId))
            {
                user.Novels.Add(novel);
                _context.SaveChanges();
            }

            return RedirectToAction(
                nameof(Detail),
                new { slug = novel.Slug });
        }

        [Authorize]
        [HttpPost]
        public IActionResult UnFollow(string novelId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.Novels)
                .FirstOrDefault(x => x.Uid == uid);

            if (user == null)
                return NotFound();

            var novel = user.Novels
                .FirstOrDefault(x => x.NovelId == novelId);

            if (novel != null)
            {
                user.Novels.Remove(novel);
                _context.SaveChanges();

                return RedirectToAction(
                    nameof(Detail),
                    new { slug = novel.Slug });
            }

            return RedirectToAction("Following");
        }

        [Authorize]
        public IActionResult Following()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
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
                    .ToList()
            };

            return View(vm);
        }
    }
}