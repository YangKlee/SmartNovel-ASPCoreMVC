namespace SmartNovel.Models.ViewModels
{
    public class DashboardAuthorStatsViewModel
    {
        public int? TotalNovels { get; set; }
        public int? PublicNovels { get; set; }
        public int? RemovedNovels { get; set; }
        public int? DraftNovels { get; set; }

        public int? TotalChapters { get; set; }
        public int? PublicChapters { get; set; }
        public int? RemovedChapters { get; set; }
        public int? DraftChapters { get; set; }

        public int? CreatorPoint { get; set; }
        public int? CpTrend { get; set; }
        public int? Ranking { get; set; }
    }
}
