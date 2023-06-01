using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataExtractor
{
	public class Movie
	{
		private int _id;
		private string _title;
		private string _posterURL;
		private string _trailer;
		private int? _rateCritic;
		private int? _rateAudience;
		private List<Platform> _platforms = new();
		private List<string> _photos = new();
		private string _description;
		private Dictionary<string, string> _characteristics = new();
		private List<Cast> _casts = new();
		private List<Review> _reviews = new();

		[JsonPropertyName("id")]
		public int Id { get => _id; set => _id = value; }

		[JsonPropertyName("title")]
		public string Title { get => _title; set => _title = value; }

		[JsonPropertyName("poster_url")]
		public string PosterURL { get => _posterURL; set => _posterURL = value; }

		[JsonPropertyName("trailer")]
		public string Trailer { get => _trailer; set => _trailer = value; }

		[JsonPropertyName("rate_critic")]
		public int? RateCritic { get => _rateCritic; set => _rateCritic = value; }

		[JsonPropertyName("rate_audience")]
		public int? RateAudience { get => _rateAudience; set => _rateAudience = value; }

		[JsonPropertyName("platforms")]
		public List<Platform> Platforms { get => _platforms; set => _platforms = value; }

		[JsonPropertyName("photos")]
		public List<string> Photos { get => _photos; set => _photos = value; }

		[JsonPropertyName("description")]
		public string Description { get => _description; set => _description = value; }

		[JsonPropertyName("characteristics")]
		public Dictionary<string, string> Characteristics { get => _characteristics; set => _characteristics = value; }

		[JsonPropertyName("casts")]
		public List<Cast> Casts { get => _casts; set => _casts = value; }

		[JsonPropertyName("reviews")]
		public List<Review> Reviews { get => _reviews; set => _reviews = value; }
	}
}
