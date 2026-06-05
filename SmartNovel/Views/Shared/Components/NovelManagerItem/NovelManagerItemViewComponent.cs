using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models.ViewModel;
namespace SmartNovel.Views.Shared.Components.NovelManagerItem
{
    public class NovelManagerItemViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(NovelViewModel model)
        {
            return View(model);
        }
    }
}
