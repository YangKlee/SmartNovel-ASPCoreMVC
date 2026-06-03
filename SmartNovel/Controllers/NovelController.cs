using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModels;
using SmartNovel.Services;
using System.Security.Claims;

namespace SmartNovel.Controllers
{
    public class NovelController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        private readonly ChapterContentService _chapterContentService;

        public NovelController(SmartTruyenDbContext context, ChapterContentService chapterContentService)
        {
            _context = context;
            _chapterContentService = chapterContentService;
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
                .Where(x =>
                    x.NovelId == novel.NovelId &&
                    x.Status == "Public")
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

        [Route("doc-truyen/{novelId}/{chapterId}")]
        public async Task<IActionResult> Read(string novelId, string chapterId)
        {
            var chapter = await _context.Chapters
                .Include(x => x.Novel)
                .FirstOrDefaultAsync(x =>
                    x.NovelId == novelId &&
                    x.ChapterId == chapterId);

            if (chapter == null)
                return NotFound();

            if (chapter.Status != "Public")
                return NotFound();

            string htmlContent =
                await _chapterContentService.GetContentAsync(
                    chapter.ChapterFileUrl);

            var prevChapter = await _context.Chapters
                .Where(x =>
                    x.NovelId == novelId &&
                    x.Status == "Public" &&
                    x.ChaperOrder < chapter.ChaperOrder)
                .OrderByDescending(x => x.ChaperOrder)
                .FirstOrDefaultAsync();

            var nextChapter = await _context.Chapters
                .Where(x =>
                    x.NovelId == novelId &&
                    x.Status == "Public" &&
                    x.ChaperOrder > chapter.ChaperOrder)
                .OrderBy(x => x.ChaperOrder)
                .FirstOrDefaultAsync();

            var vm = new ReadNovelVM
            {
                NovelId = chapter.NovelId,

                ChapterId = chapter.ChapterId,

                NovelTitle = chapter.Novel.Title,

                ChapterTitle = chapter.ChapterTitle,

                ChapterOrder = chapter.ChaperOrder,

                HtmlContent = htmlContent,

                PrevChapterId = prevChapter?.ChapterId,

                PrevChapterTitle = prevChapter?.ChapterTitle,

                NextChapterId = nextChapter?.ChapterId,

                NextChapterTitle = nextChapter?.ChapterTitle
            };

            return View(vm);
        }
    }
}