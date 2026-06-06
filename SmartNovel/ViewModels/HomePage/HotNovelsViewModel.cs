using Microsoft.AspNetCore.Mvc;
using SmartNovel.Models;

namespace SmartNovel.ViewModels
{
    public class HotNovelsViewModel
    {
        public List<Novel> Daily { get; set; } = new List<Novel>();

        public List<Novel> Monthly { get; set; } =new List<Novel> { };

        public List<Novel> Quarterly { get; set; } =new List<Novel>{ };
        public List<Novel> Sesonaly { get; set; } =new List<Novel>{ };
    }
       
}
