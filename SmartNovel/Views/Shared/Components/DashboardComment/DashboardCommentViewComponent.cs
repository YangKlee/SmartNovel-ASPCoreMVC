using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;
using SmartNovel.Services;
namespace SmartNovel.Views.Shared.Components.DashboardComment
{
    public class DashboardCommentViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<Comment> cmts)
        {

            return View(cmts);
        }
    }
}
