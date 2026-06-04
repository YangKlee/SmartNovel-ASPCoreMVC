using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models.ViewModel;
namespace SmartNovel.Views.Shared.Components.NovelManagerItem
{
    public class NovelManagerItem : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(NovelManagermentViewModel model)
        {
            return View(model);
        }
    }
}
