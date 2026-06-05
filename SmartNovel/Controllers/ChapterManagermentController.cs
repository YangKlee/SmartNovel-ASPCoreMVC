using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using SmartNovelBE.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
namespace SmartNovel.Controllers
{
    [Authorize(Roles = "3")]
    public class ChapterManagermentController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        private readonly FileStorageServices _fileServicesUpload;
        private  readonly HttpClient _httpClient = new HttpClient();
        public ChapterManagermentController(SmartTruyenDbContext context, FileStorageServices fileServicesUpload, HttpClient httpClient )
        {
            _context = context;
            _fileServicesUpload = fileServicesUpload;
            _httpClient = httpClient;
        }
        [HttpGet("ChapterManagerment/Index/{novelId}")]
        public async Task<IActionResult> Index(ChapterManagermentViewModel req, string? novelId)
        {

            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid == null) return Redirect("/auth/Login");

            // Nếu truy cập từ nút "Quản lý chương" có chứa novelId trên URL
            if (!string.IsNullOrEmpty(novelId) && string.IsNullOrEmpty(req.SelectedNovelId))
            {
                req.SelectedNovelId = novelId;
            }

            // Lấy danh sách truyện và đếm số lượng thống kê chương của mỗi truyện
            var novels = await _context.Novels
                .AsNoTracking()
                .Where(n => n.Uid == uid)
                .Select(n => new NovelViewModel
                {
                    NovelId = n.NovelId,
                    Title = n.Title,
                    countChapter = n.Chapters.Count(),
                    countChapterPublic = n.Chapters.Count(c => c.Status == "Public"),
                    countChapterDraft = n.Chapters.Count(c => c.Status == "Draft"),
                    countChapterRemove = n.Chapters.Count(c => c.Status == "Cancel")
                })
                .ToListAsync();

            req.Novels = novels;

                // Mặc định chọn truyện đầu tiên nếu chưa chọn
            if (string.IsNullOrEmpty(req.SelectedNovelId) && novels.Any())
            {
                req.SelectedNovelId = novels.First().NovelId;
            }


            req.SelectedNovel = novels.FirstOrDefault(n => n.NovelId == req.SelectedNovelId);


            var chaptersQuery = _context.Chapters.AsNoTracking().Where(c => c.NovelId == req.SelectedNovelId);

            if (!string.IsNullOrEmpty(req.Keyword))
            {
                chaptersQuery = chaptersQuery.Where(c => c.ChapterTitle.Contains(req.Keyword) || (c.SummaryChapter != null && c.SummaryChapter.Contains(req.Keyword)));
            }

            if (!string.IsNullOrEmpty(req.SelectedStatus) && req.SelectedStatus.ToLower() != "all")
            {
                string filterStatus = req.SelectedStatus;
                // Chuẩn hóa case do Model lưu giá trị có viết hoa chữ cái đầu
                if (filterStatus.ToLower() == "public") filterStatus = "Public";
                if (filterStatus.ToLower() == "draft") filterStatus = "Draft";
                if (filterStatus.ToLower() == "cancel") filterStatus = "Cancel";

                chaptersQuery = chaptersQuery.Where(c => c.Status == filterStatus);
            }

            var totalCount = await chaptersQuery.CountAsync();
            var chapters = await chaptersQuery
                .OrderByDescending(c => c.ChaperOrder)
                .Skip((req.CurrentPage - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            req.chapters = chapters;
            req.TotalItem = totalCount;

            return View(req);
        }
        [HttpGet("ChapterManagerment/Add/{novelID}")]
        [Authorize(Roles ="3")]
        public async Task<IActionResult> Add(string novelId)
        {
            if (novelId == null)
                return NotFound();
            return View();
        }

        [HttpPost("ChapterManagerment/Add/{novelID}")]
        [Authorize(Roles ="3")]
        public async Task<IActionResult> Add(string novelId, CreateChapterViewModel req)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Vui lòng kiểm tra lại thông tin.";
                return View(req);
            }

            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ehe = await _context.Novels.AnyAsync(n => n.NovelId == novelId && n.Uid == uid);
            if (!ehe)
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Cưng tính copy id truyện người ta rồi gửi request đăng chương à, đâu có dễ dị";
                return View(req);
            }
            if(await checkDublicateOrderChapter(novelId, req.Order) == true)
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Số chương đã tồn tại";
                return View(req);
            }
            var newChapter = new Chapter();
            string idChapter = Guid.NewGuid().ToString();
            newChapter.ChapterTitle = req.Title;
            newChapter.ChapterId = idChapter;
            newChapter.SummaryChapter = req.Description;
            newChapter.AllowComment = req.AllowComment;
            newChapter.Status = req.Status;
            newChapter.UpdateTime = DateTime.Now;
            newChapter.CreateTime = DateTime.Now;
            newChapter.NovelId = novelId;
            newChapter.ChaperOrder = req.Order;

            try
            {
                // upload file
                string publicLink = "https://pub-20056e4912f440f08b3d40eea545f95f.r2.dev/smart-novel/novel-file/";

                string fileName = $"{Guid.NewGuid().ToString()}-{idChapter}.html";
                var res = await _fileServicesUpload.UploadHtml("smart-novel/novel-file/", fileName, req.Content);
                if (res)
                    newChapter.ChapterFileUrl = publicLink + fileName;
                
                _context.Chapters.Add(newChapter);
                await _context.SaveChangesAsync();

                TempData["Success"] = true;
                TempData["Msg"] = "Thêm chương mới thành công!";
                // Chuyển hướng sang trang Index
                return RedirectToAction("Index", new { novelId = novelId });
            }
            catch
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Đã xảy ra lỗi hệ thống khi lưu dữ liệu.";
                return View(req);
            }
        }
        public async Task<bool> checkDublicateOrderChapter(string novelID,int order)
        {
            return await _context.Chapters.AnyAsync(c => c.NovelId == novelID && c.ChaperOrder == order);
        }
        [HttpGet("ChapterManagerment/Modify/{novelID}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> modify(string? novelID)
        {
            if (novelID == null)
                return NotFound();
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ehe = await _context.Novels.AnyAsync(n => n.NovelId == novelID && n.Uid == uid);
            if (!ehe)
            {
                ViewBag.Success = false;
                ViewBag.Msg = "Cưng tính copy id truyện người ta rồi sửa chương à, t rào trước r";
                return View("Add", new CreateChapterViewModel());
            }
            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.NovelId == novelID);
            string htmlContent = "";
            try
            {

                htmlContent = await _httpClient.GetStringAsync(chapter.ChapterFileUrl);

            }
            catch (HttpRequestException e)
            {
                htmlContent = "Xảy ra lỗi khi lấy file truyện từ máy chủ";
            }
            var model = new CreateChapterViewModel
            {
                Title = chapter?.ChapterTitle,
                AllowComment = chapter.AllowComment ?? false,
                Description = chapter?.SummaryChapter,
                Order = chapter.ChaperOrder,
                Status = chapter.Status,
                Content = htmlContent

            };
            ViewBag.isEdit = true;
            return View("Add", model);
        }
    }
}
