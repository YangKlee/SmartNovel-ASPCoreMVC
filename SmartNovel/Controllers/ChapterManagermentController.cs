using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using System.Security.Claims;

namespace SmartNovel.Controllers
{
    [Authorize(Roles = "3")]
    public class ChapterManagermentController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public ChapterManagermentController(SmartTruyenDbContext context)
        {
            _context = context;
        }

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
    }
}
