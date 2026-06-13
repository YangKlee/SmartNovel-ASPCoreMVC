using SmartNovel.Models.ViewModel;
using System.Collections.Generic;

namespace SmartNovel.Services
{
    public class QuickToolService
    {
        public List<QuickToolItemViewModel> GetQuickToolsByRole(string roleId)
        {
            var tools = new List<QuickToolItemViewModel>();

            switch (roleId)
            {
                case "1": // Admin

                    tools.Add(new QuickToolItemViewModel { Content = "Quản lý truyện", IconBootstrap = "bi-folder-fill", Link = "/NovelManagerment" });
                    tools.Add(new QuickToolItemViewModel { Content = "Quản lý bình luận", IconBootstrap = "bi-list", Link = "/CommentManagerment/" });
                    tools.Add(new QuickToolItemViewModel { Content = "Quản lý người dùng", IconBootstrap = "bi-list", Link = "/AdminUser/" });
                    break;
                case "2": // Moderator
                    tools.Add(new QuickToolItemViewModel { Content = "Quản lý truyện", IconBootstrap = "bi-folder-fill", Link = "/NovelManagerment" });
                    tools.Add(new QuickToolItemViewModel { Content = "Quản lý bình luận", IconBootstrap = "bi-list", Link = "/CommentManagerment/" });

                    break;
                case "3": // Author
                    tools.Add(new QuickToolItemViewModel { Content = "Truyện của tôi", IconBootstrap = "bi-folder-fill", Link = "/NovelManagerment" });
                    tools.Add(new QuickToolItemViewModel { Content = "Thêm truyện mới", IconBootstrap = "bi-folder-plus", Link = "/NovelManagerment/Add" });
                    tools.Add(new QuickToolItemViewModel { Content = "Bình luận", IconBootstrap = "bi bi-chat", Link = "/CommentReader" });
         ;
                    break;
                default:
                    // Provide empty or default tools for ordinary users
                    break;
            }

            return tools;
        }
    }
}
