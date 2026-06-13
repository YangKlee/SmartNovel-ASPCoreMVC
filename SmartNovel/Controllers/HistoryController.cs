using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using System.Security.Claims;

namespace SmartNovel.Controllers
{
    public class HistoryController : Controller
    {
        private readonly SmartTruyenDbContext _context;
        public HistoryController(SmartTruyenDbContext context) { _context = context; }
        
        public async Task<IActionResult> Index()
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(uid))
            {
                return Redirect("/Auth/Login"); // Or handle unauthorized access
            }

            var history = await _context.HistoryReaders
                    .Include(h => h.Novel)
                    .Include(h => h.Chapter)
                    .Where(h => h.Uid == uid)
                    .OrderByDescending(h => h.TimeReader)
                    .ToListAsync();

            var model = history.Select(h => new HistoryViewModel
            {
                history = new NovelHistoryViewModel
                {
                    chapterView = h.Chapter,
                    novelInfo = h.Novel
                },
                timeView = h.TimeReader
            }).ToList();

            return View(model);
        }
    }
}
