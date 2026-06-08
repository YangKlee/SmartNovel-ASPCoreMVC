namespace SmartNovel.Views.Shared.Components.CommentComponent
{
    using Microsoft.AspNetCore.Mvc;
    using SmartNovel.Models;

    public class CommentViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Comment model)
        {
            return View(model);
        }
    }
}
