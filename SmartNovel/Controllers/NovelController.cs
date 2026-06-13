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
            bool isFollowingAuthor = false;
            // xác định xem đã follow author hay chưa
            
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
                    .Include(x => x.FollowerUs)
                    .FirstOrDefault(x => x.Uid == uid);

                if (user != null)
                {
                    isAuthorBlocked = user.Authors
                        .Any(x => x.Uid == novel.Uid);
                    isFollowingAuthor = user.FollowerUs
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

                IsFollowingAuthor = isFollowingAuthor,

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
            var comment = await _context.Comments.Include(c=> c.UidNavigation).Where(c => c.ChapterId == chapter.ChapterId).ToListAsync();
            if (chapter == null)
                return NotFound();
            var role = User.FindFirstValue(ClaimTypes.Role); // mài định chặn hem cho tác giả coi truyện hay gì.đã sửa
            if (chapter.Status.ToLower() != "public" && role == "4")
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
                Comments = comment,
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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromForm] string ChapterID, [FromForm] string NovelID, [FromForm] string Content)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var newComment = new Comment
            {
                ChapterId = ChapterID,
                CommentId = Guid.NewGuid().ToString(),
                Content = Content,
                TimeCommeny = DateTime.Now,
                Uid = uid,
                Status = "Active",
              

            };
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            return Redirect($"/truyen/{NovelID}/{ChapterID}#{newComment.CommentId}");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment([FromForm] string commentId, [FromForm] string chapterId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId && c.ChapterId == chapterId);
            if (comment == null)
            {
                return NotFound();
            }
            // Chỉ chủ ở hữu comment mới được xoá (trừ khi là Admin hoặc Mod)
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (comment.Uid != uid && role != "1" && role != "2")
            {
                return Forbid();
            }
            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.ChapterId == chapterId);
            if (chapter == null)
            {
                return NotFound();
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Redirect($"/truyen/{chapter.NovelId}/{chapterId}#comment-container");
        }
        [HttpPost]
        [Authorize(Roles ="3,4")]
        public async Task<IActionResult> followAuthor([FromForm] string authorId, [FromForm] string novelID)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var author = await _context.Users.FirstOrDefaultAsync(u => u.Uid == authorId);
            var user = await _context.Users.Include(u => u.FollowerUs).FirstOrDefaultAsync(u => u.Uid == uid);
            
            if (author == null || user == null)
                return NotFound();
            user.FollowerUs.Add(author);
            await _context.SaveChangesAsync();
            return Redirect($"/truyen/{novelID}");

        }
        [HttpPost]
        [Authorize(Roles = "3,4")]
        public async Task<IActionResult> unFollowAuthor([FromForm] string authorId, [FromForm] string? novelID)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var author = await _context.Users.FirstOrDefaultAsync(u => u.Uid == authorId);
            var user = await _context.Users.Include(u => u.FollowerUs).FirstOrDefaultAsync(u => u.Uid == uid);

            if (author == null || user == null)
                return NotFound();
            user.FollowerUs.Remove(author);
            await _context.SaveChangesAsync();
            return Redirect($"/truyen/{novelID}");

        }

    }
}