using System.Text.Json.Serialization;

namespace CineScrapeProject.wwwroot.Models
{
	public class Platform
	{
		private string _image;
		private string _name;
		private static Dictionary<string, string> _platformsNames = new()
		{
			{"in-theaters", "Cinema"},
			{"vudu", "Vudu"},
			{"peacock", "Peacock"},
			{"netflix", "Netflix"},
			{"hulu", "Hulu"},
			{"amazon-prime-video-us", "Prime Video"},
			{"disney-plus-us", "Disney +"},
			{"hbo-max", "HBO Max"},
			{"paramount-plus-us", "Paramount +"},
			{"apple-tv-plus-us", "TV +"},
			{"showtime", "Showtime"},
			{"itunes", "Tv"}
		};

		[JsonPropertyName("image")]
		public string Image { get => _image; set => _image = value; }

		[JsonPropertyName("name")]
		public string Name { get => _name; set => _name = value; }
		public static Dictionary<string, string> PlatformsNames { get => _platformsNames; set => _platformsNames = value; }
	}
}
