using Microsoft.AspNetCore.Authorization;
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
        private readonly SmartNovelBE.Services.FileStorageServices _fileServicesUpload;
        
        public NovelManagermentController(SmartTruyenDbContext context, SmartNovelBE.Services.FileStorageServices fileServicesUpload)
        {
            _context = context;
            _fileServicesUpload = fileServicesUpload;
        }
        [Authorize(Roles = "3")]
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
            // đống này lộn xộn vcl nhưng miễn nó hoạt động đc là oke
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
        [HttpGet]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Add()
        {
            var cate = await _context.Categories.Where(c => c.Status == "active").ToListAsync();
            ViewBag.AvailableGenres = cate;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Add(CreateNovelViewModel req)
        {
            var cate = await _context.Categories.Where(c => c.Status == "active").ToListAsync();
            ViewBag.AvailableGenres = cate;

            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(req);
            }

            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid == null) return Redirect("/auth/Login");

            if (req.Status != "Public" && req.Status != "Draft")
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Trạng thái truyện không hợp lệ.";
                return View(req);
            }

            var newNovel = new Novel();
            Guid idNovel = Guid.NewGuid();
            newNovel.NovelId = idNovel.ToString();
            newNovel.Status = req.Status;
            newNovel.Title = req.Title;

            newNovel.Slug = idNovel.ToString();
            newNovel.Description = req.Description;
            newNovel.UpdateTime = DateTime.Now;
            newNovel.CreateTime = DateTime.Now;
            newNovel.AgeRating = req.AgeRating;
            newNovel.ImageNovelUrl = "";
            newNovel.ImageBanerNovelUrl = "";
            newNovel.Uid = uid;

            if (req.Genres != null)
            {
                foreach (var categoryId in req.Genres)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
                    if (category != null)
                    {
                        newNovel.Categories.Add(category);
                    }
                }
            }

            try
            {
                _context.Novels.Add(newNovel);
                await _context.SaveChangesAsync();

                string publicLink = "https://pub-20056e4912f440f08b3d40eea545f95f.r2.dev/smart-novel/novel-image/";
                Guid idFile = Guid.NewGuid();
                Guid idFile1 = Guid.NewGuid();

                var novel1 = await _context.Novels.FirstOrDefaultAsync(n => n.NovelId == idNovel.ToString());

                if (req.CoverImage != null)
                {
                    string fileCoverNameRaw = req.CoverImage.FileName;
                    string fileCoverName = $"{idFile.ToString()}-{fileCoverNameRaw}";
                    var resultUploadCover = await _fileServicesUpload.UploadFile("smart-novel/novel-image/",
                        fileCoverName, req.CoverImage);

                    if (resultUploadCover && novel1 != null)
                    {
                        novel1.ImageNovelUrl = publicLink + fileCoverName;
                    }
                }

                if (req.BannerImage != null)
                {
                    string fileBannerNameRaw = req.BannerImage.FileName;
                    string fileBannerName = $"{idFile1.ToString()}-{fileBannerNameRaw}";

                    var resultUploadBanner = await _fileServicesUpload.UploadFile("smart-novel/novel-image/",
                        fileBannerName, req.BannerImage);
                    if (resultUploadBanner && novel1 != null)
                    {
                        novel1.ImageBanerNovelUrl = publicLink + fileBannerName;
                    }
                }

                await _context.SaveChangesAsync();
                
                ViewBag.Success = true;
                ViewBag.Msg = "Tạo truyện thành công!";
                ModelState.Clear();
                return View(new CreateNovelViewModel()); 
            }
            catch
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Something went wrong huhuhuuhhu";
                return View(req);
            }
        }
    }
}
