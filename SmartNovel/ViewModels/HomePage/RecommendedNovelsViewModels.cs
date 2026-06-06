using SmartNovel.Models;
using System.Collections.Generic;

namespace SmartNovel.ViewModels
{
	public class RecommendedViewModel
	{
		public List<Novel> Daily { get; set; } = new List<Novel>();
		public List<Novel> Weekly { get; set; } = new List<Novel>();
		public List<Novel> Monthly { get; set; } = new List<Novel>();
	}
}