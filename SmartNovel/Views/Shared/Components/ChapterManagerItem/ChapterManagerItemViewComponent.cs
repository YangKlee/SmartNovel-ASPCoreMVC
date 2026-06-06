using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;
namespace SmartNovel.Views.Shared.Components.ChapterManagerItem
{
    public class ChapterManagerItemViewComponent:  ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Chapter model)
        {
            return View(model);
        }
    }
}
