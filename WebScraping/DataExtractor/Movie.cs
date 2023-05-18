using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor
{
	public class Movie
	{
		private string _title;
		private int _rateCritic;
		private int _rateAudience;
		private string _description;
		private Dictionary<string, string> _characteristics = new();

		public string Title { get; set; }
		public int RateCritic { get; set; }
		public int RateAudience { get; set; }
		public string Description { get; set; }
		public Dictionary<string, string> Characteristics { get => _characteristics; set => _characteristics = value; }
	}
}
