using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;
using SmartNovel.Models.ViewModels;

namespace SmartNovel.Views.Shared.Components.DashboardAuthorStats
{
    public class DashboardAuthorStatsViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(DashboardAuthorStatsViewModel model)
        {

            return View(model);
        }
    }
}
