using SmartNovel.Models;
using SmartNovel.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartNovel.Services
{
    public class MenuDashboardServices
    {
        private readonly SmartTruyenDbContext _context;

        public MenuDashboardServices(SmartTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<MenuDashboardViewModel> GetMenuDashboardAsync(ClaimsPrincipal user)
        {
            var model = new MenuDashboardViewModel
            {
                FullName = "Khách",
                Role = "Guest",
                AvatarUrl = string.Empty,
                items = new List<MenuDashboardItem>()
            };

            if (user?.Identity?.IsAuthenticated == true)
            {
                model.FullName = user.Identity?.Name ?? "Ẩn danh";
                var roleId = user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
                model.Role = GetRoleName(roleId);
                model.items = GetMenuItemsByRoleId(roleId);

                var uid = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(uid))
                {
                    var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Uid == uid);
                    if (appUser != null)
                    {
                        model.AvatarUrl = appUser.AvartarUrl ?? string.Empty;
                    }
                }
            }

            return model;
        }

        private string GetRoleName(string roleId)
        {
            return roleId switch
            {
                "1" => "Admin",
                "2" => "Moderator",
                "3" => "Author",
                _ => "User"
            };
        }

        private List<MenuDashboardItem> GetMenuItemsByRoleId(string roleId)
        {
            var items = new List<MenuDashboardItem>();

            switch (roleId)
            {
                case "1": // Admin
                    items.Add(new MenuDashboardItem { slot = 1, icon = "bi bi-house", content = "Trang chính", controllerLink = "Dashboard", actionLink = "Index" });
                    items.Add(new MenuDashboardItem { slot = 1, icon = "bi bi-people", content = "Quản lý User", controllerLink = "AdminUser", actionLink = "Index" });
                    items.Add(new MenuDashboardItem { slot = 2, icon = "bi bi-tags", content = "Quản lý Thể loại", controllerLink = "AdminCategory", actionLink = "Index" });
                    items.Add(new MenuDashboardItem { slot = 3, icon = "bi bi-book", content = "Quản lý Truyện", controllerLink = "NovelManagerment", actionLink = "Index" });
                                    items.Add(new MenuDashboardItem { slot = 3, icon = "bi bi-chat", content = "Quản lý bình luận", controllerLink = "CommentManagerment", actionLink = "Index" });

                    break;
                case "2": // Moderator
                    items.Add(new MenuDashboardItem { slot = 1, icon = "bi bi-house", content = "Trang chính", controllerLink = "Dashboard", actionLink = "Index" });
                    items.Add(new MenuDashboardItem { slot = 2, icon = "bi bi-list-task", content = "Quản lý truyện", controllerLink = "NovelManagerment", actionLink = "Index" });
                    items.Add(new MenuDashboardItem { slot = 3, icon = "bi bi-chat", content = "Quản lý bình luận", controllerLink = "CommentManagerment", actionLink = "Index" });
                    break;
                case "3": // Author
                    items.Add(new MenuDashboardItem { slot = 1, icon = "bi bi-house", content = "Trang chính", controllerLink = "Dashboard", actionLink = "Index" });
                    items.Add(new MenuDashboardItem { slot = 1, icon = "bi bi-journal-plus", content = "Đăng truyện mới", controllerLink = "NovelManagerment", actionLink = "Add" });
                    items.Add(new MenuDashboardItem { slot = 2, icon = "bi bi-list-task", content = "Truyện của tôi", controllerLink = "NovelManagerment", actionLink = "Index" });
                    items.Add(new MenuDashboardItem { slot = 3, icon = "bi bi-chat", content = "Bình luận độc giả", controllerLink = "CommentReader", actionLink = "Index" });
                    break;
                default:
                    // Menu mặc định cho User thường
                    items.Add(new MenuDashboardItem { slot = 1, icon = "bi bi-person", content = "Hồ sơ cá nhân", controllerLink = "Account", actionLink = "Profile" });
                    break;
            }

            return items;
        }
    }
}
