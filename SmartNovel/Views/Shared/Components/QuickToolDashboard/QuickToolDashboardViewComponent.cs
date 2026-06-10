using Microsoft.AspNetCore.Mvc;
using SmartNovel.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartNovel.ViewComponents
{
    public class QuickToolDashboardViewComponent : ViewComponent
    {
        private readonly QuickToolService _quickToolService;

        public QuickToolDashboardViewComponent(QuickToolService quickToolService)
        {
            _quickToolService = quickToolService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "4"; // Default role if not found
            var tools = _quickToolService.GetQuickToolsByRole(roleId);
            return View(tools);
        }
    }
}
