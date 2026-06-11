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

        [Route("truyen/{novelId}")]
        public IActionResult Detail(string novelId)
        {
            if (string.IsNullOrEmpty(novelId))
                return NotFound();

            var novel = _context.Novels
                .Include(x => x.UidNavigation)
                .Include(x => x.Categories)
                .Include(x => x.Uids)
                .FirstOrDefault(x => x.NovelId == novelId);

            if (novel == null)
                return NotFound();

            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isFollowing = false;
            bool isAuthorBlocked = false;
            // Đanh giá trung bình và đánh giá của người dùng
            var ratings = _context.Ratings
            .Where(x => x.NovelId == novelId)
            .ToList();

            double avgRating = ratings.Any()
                ? ratings.Average(x => x.RatingPoint)
                : 0;

            double? userRating = null;

            if (!string.IsNullOrEmpty(uid))
            {
                userRating = ratings
                    .FirstOrDefault(x => x.Uid == uid)
                    ?.RatingPoint;
            }

            if (!string.IsNullOrEmpty(uid))
            {
                var user = _context.Users
                    .Include(x => x.Authors)
                    .FirstOrDefault(x => x.Uid == uid);

                if (user != null)
                {
                    isAuthorBlocked = user.Authors
                        .Any(x => x.Uid == novel.Uid);
                }
            }

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

                IsAuthorBlocked = isAuthorBlocked,

                FollowCount = novel.Uids.Count,

                AverageRating = avgRating,

                UserRating = userRating,

                TotalRatings = ratings.Count
            };
            var currentNovel = novel;

            // Lấy danh sách thể loại của truyện hiện tại
            var categoryIds = currentNovel.Categories
                .Select(x => x.CategoryId)
                .ToList();

            // Truyện đề xuất
            var recommendedNovels = _context.Novels
                .Include(x => x.UidNavigation)
                .Include(x => x.Categories)
                .Where(x =>
                    x.NovelId != currentNovel.NovelId &&
                    x.Status.ToLower() == "public" && 
                    x.Categories.Any(c => categoryIds.Contains(c.CategoryId)))
                .OrderByDescending(x => x.ViewCount)
                .Take(5)
                .ToList();

            vm.RecommendedNovels = recommendedNovels;
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
                new { novelId = novel.NovelId });
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
                    new { novelId = novel.NovelId });
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

        [Route("truyen/{novelId}/{chapterId}")]
        public async Task<IActionResult> Read(string novelId, string chapterId)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Novel)
                .FirstOrDefaultAsync(x =>
                    x.NovelId == novelId &&
                    x.ChapterId == chapterId);

            if (chapter == null)
                return NotFound();

            if (chapter.Status.ToLower() != "public")
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

                NextChapterTitle = nextChapter?.ChapterTitle,

                AllChapters = await _context.Chapters
                .Where(x =>
                    x.NovelId == novelId &&
                    x.Status == "Public")
                .OrderBy(x => x.ChaperOrder)
                .ToListAsync()
            };

            return View(vm);
        }

        // Đánh giá truyện
        [Authorize]
        [HttpPost]
        public IActionResult Rate(string novelId, double rating)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (uid == null)
                return Unauthorized();

            var existing = _context.Ratings
                .FirstOrDefault(x =>
                    x.Uid == uid &&
                    x.NovelId == novelId);

            if (existing == null)
            {
                _context.Ratings.Add(new Rating
                {
                    Uid = uid,
                    NovelId = novelId,
                    RatingPoint = rating
                });
            }
            else
            {
                existing.RatingPoint = rating;
            }

            _context.SaveChanges();

            return RedirectToAction(
                nameof(Detail),
                new { novelId });
        }
        //Chặn tác giả
        [Authorize]
        [HttpPost]
        public IActionResult BlockAuthor(string authorId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.Authors)
                .FirstOrDefault(x => x.Uid == uid);

            var author = _context.Users
                .FirstOrDefault(x => x.Uid == authorId);

            if (user == null || author == null)
                return NotFound();

            if (!user.Authors.Any(x => x.Uid == authorId))
            {
                user.Authors.Add(author);
                _context.SaveChanges();
            }

            if (uid == authorId)
            {
                return RedirectToAction("Detail",
                    new { novelId = Request.Form["novelId"] });
            }

            return RedirectToAction(
                nameof(BlockedAuthors));
        }

        // Bỏ chặn tác giả
        [Authorize]
        [HttpPost]
        public IActionResult UnBlockAuthor(string authorId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.Authors)
                .FirstOrDefault(x => x.Uid == uid);

            if (user == null)
                return NotFound();

            var author = user.Authors
                .FirstOrDefault(x => x.Uid == authorId);

            if (author != null)
            {
                user.Authors.Remove(author);
                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        [Authorize]
        public IActionResult BlockedAuthors()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users
                .Include(x => x.Authors)
                .FirstOrDefault(x => x.Uid == uid);

            if (user == null)
                return NotFound();

            var vm = new BlockedAuthorVM
            {
                Authors = user.Authors
                    .OrderBy(x => x.DisplayName)
                    .ToList()
            };

            return View(vm);
        }
    }
}