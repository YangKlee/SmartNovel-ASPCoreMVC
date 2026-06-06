namespace SmartNovel.Models.ViewModel
{
    public class MenuDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<MenuDashboardItem> items { set; get; }
    }
}
