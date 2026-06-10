using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;

namespace SmartNovel.Controllers
{
    [Authorize(Roles = "1,2")]
    public class CommentManagermentController : Controller
    {
        private readonly SmartTruyenDbContext _context;

        public CommentManagermentController(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(CommentManagermentViewModel req)
        {
            if (string.IsNullOrEmpty(req.Keyword))
            {
                req.Comments = new List<Comment>();
                req.TotalItem = 0;
                return View(req);
            }

            var query = _context.Comments
                .Include(c => c.UidNavigation)
                .Include(c => c.Chapter)
                .AsNoTracking()
                .AsQueryable();

            query = query.Where(c => c.Content.Contains(req.Keyword) || 
                                     c.UidNavigation.DisplayName.Contains(req.Keyword) || 
                                     c.UidNavigation.Username.Contains(req.Keyword));

            var totalCount = await query.CountAsync();
            
            var comments = await query
                .OrderByDescending(c => c.TimeCommeny)
                .Skip((req.CurrentPage - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            req.Comments = comments;
            req.TotalItem = totalCount;

            return View(req);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = true;
                TempData["Msg"] = "Đã xóa bình luận thành công!";
            }
            else
            {
                TempData["Success"] = false;
                TempData["Msg"] = "Không tìm thấy bình luận.";
            }

            return RedirectToAction("Index");
        }
    }
}
