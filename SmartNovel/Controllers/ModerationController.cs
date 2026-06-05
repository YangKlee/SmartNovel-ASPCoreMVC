using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;

namespace SmartNovel.Controllers
{
    public class ModerationController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public ModerationController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // TRANG QUẢN LÝ BÁO CÁO
        // =====================================================
        public async Task<IActionResult> Index()
        {
            ViewBag.Novels = await _context.ReportTickets
                .Include(x => x.Novel)
                .Where(x =>
                    x.Type == "NOVEL" &&
                    x.Status == "PENDING")
                .OrderByDescending(x => x.TimeSend)
                .ToListAsync();

            ViewBag.Chapters = await _context.ReportTickets
                .Include(x => x.Chapter)
                .ThenInclude(x => x.Novel)
                .Where(x =>
                    x.Type == "CHAPTER" &&
                    x.Status == "PENDING")
                .OrderByDescending(x => x.TimeSend)
                .ToListAsync();

            ViewBag.Comments = await _context.ReportTickets
                .Include(x => x.Comment)
                .ThenInclude(x => x.Chapter)
                .ThenInclude(x => x.Novel)
                .Where(x =>
                    x.Type == "COMMENT" &&
                    x.Status == "PENDING")
                .OrderByDescending(x => x.TimeSend)
                .ToListAsync();

            ViewBag.History = await _context.ReportTickets
                .Include(x => x.RepoterU)
                .Include(x => x.ResolvedU)
                .Where(x =>
                    x.Status == "RESOLVED" ||
                    x.Status == "REJECTED")
                .OrderByDescending(x => x.TimeSend)
                .ToListAsync();

            return View();
        }

        // =====================================================
        // GỠ TRUYỆN
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveNovel(string ticketId)
        {
            var ticket = await _context.ReportTickets
                .FirstOrDefaultAsync(x =>
                    x.TiketId == ticketId &&
                    x.Type == "NOVEL" &&
                    x.Status == "PENDING");

            if (ticket == null)
                return NotFound();

            var novel = await _context.Novels
                .FirstOrDefaultAsync(x =>
                    x.NovelId == ticket.NovelId);

            if (novel == null)
                return NotFound();

            novel.Status = "REMOVED";

            ticket.Status = "RESOLVED";
            ticket.ResolvedUid = "U002";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã gỡ truyện thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GỠ CHƯƠNG
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveChapter(string ticketId)
        {
            var ticket = await _context.ReportTickets
                .FirstOrDefaultAsync(x =>
                    x.TiketId == ticketId &&
                    x.Type == "CHAPTER" &&
                    x.Status == "PENDING");

            if (ticket == null)
                return NotFound();

            var chapter = await _context.Chapters
                .FirstOrDefaultAsync(x =>
                    x.ChapterId == ticket.ChapterId);

            if (chapter == null)
                return NotFound();

            chapter.Status = "REMOVED";

            ticket.Status = "RESOLVED";
            ticket.ResolvedUid = "U002";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã gỡ chương thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GỠ BÌNH LUẬN
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveComment(string ticketId)
        {
            var ticket = await _context.ReportTickets
                .FirstOrDefaultAsync(x =>
                    x.TiketId == ticketId &&
                    x.Type == "COMMENT" &&
                    x.Status == "PENDING");

            if (ticket == null)
                return NotFound();

            var comment = await _context.Comments
                .FirstOrDefaultAsync(x =>
                    x.CommentId == ticket.CommentId);

            if (comment == null)
                return NotFound();

            comment.Status = "REMOVED";

            ticket.Status = "RESOLVED";
            ticket.ResolvedUid = "U002";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã gỡ bình luận thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // BÁC BỎ BÁO CÁO
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectTicket(string ticketId)
        {
            var ticket = await _context.ReportTickets
                .FirstOrDefaultAsync(x =>
                    x.TiketId == ticketId &&
                    x.Status == "PENDING");

            if (ticket == null)
                return NotFound();

            ticket.Status = "REJECTED";
            ticket.ResolvedUid = "U002";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã bác bỏ báo cáo.";

            return RedirectToAction(nameof(Index));
        }
    }
}