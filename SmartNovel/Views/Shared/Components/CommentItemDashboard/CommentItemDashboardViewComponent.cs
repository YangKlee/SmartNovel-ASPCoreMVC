using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;
namespace SmartNovel.Views.Shared.Components.CommentItemDashboard
{
    public class CommentItemDashboardViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Comment model)
        {
            return View(model);
        }
    }
}
