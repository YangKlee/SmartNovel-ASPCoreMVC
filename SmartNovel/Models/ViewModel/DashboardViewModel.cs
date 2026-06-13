using SmartNovel.Models.ViewModels;

namespace SmartNovel.Models.ViewModel
{
    public class DashboardViewModel
    {
        public DashboardAuthorStatsViewModel authorStats { set; get; }
        public  List<Comment> lastComment { set; get; }

    }
}
