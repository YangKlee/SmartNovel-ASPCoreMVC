using Microsoft.AspNetCore.Mvc;
using SmartNovel.Services;
using System.Threading.Tasks;

namespace SmartNovel.Views.Shared.Components.MenuDashboard
{
    public class MenuDashboardViewComponent : ViewComponent
    {
        private readonly MenuDashboardServices _menuDashboardServices;

        public MenuDashboardViewComponent(MenuDashboardServices menuDashboardServices)
        {
            _menuDashboardServices = menuDashboardServices;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var claimsPrincipal = User as System.Security.Claims.ClaimsPrincipal;
            var model = await _menuDashboardServices.GetMenuDashboardAsync(claimsPrincipal);

            return View(model); 
        }
    }
}
