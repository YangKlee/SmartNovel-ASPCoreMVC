using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;

namespace SmartNovel.ViewModels
{
    public class HotNovelsViewModel
    {
        public List<Novel> Weekly { get; set; } = new List<Novel>();
        public List<Novel> Monthly { get; set; } = new List<Novel>();
    }
       
}
