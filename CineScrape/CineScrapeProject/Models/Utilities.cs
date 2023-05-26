using CineScrapeProject.Pages;
using CineScrapeProject.wwwroot.Models;

namespace CineScrapeProject.Models
{
	public static class Utilities
	{
		public static readonly string PATH = "sample-data/movies.json";

		public static string NullCritic(this int? num) => num.HasValue ? num.Value.ToString() : "--";

		public static List<Slot> PlatformFilter(this IEnumerable<Platform> source)
		{
			Dictionary<string, int> dictionary = new();

			foreach (Platform item in source)
			{
				string key = Platform.PlatformsNames[item.Name];

				if (dictionary.TryGetValue(key, out int value))
				{
					dictionary[key] = value + 1;
				}
				else
				{
					dictionary.Add(key, 1);
				}
			}

			List<Slot> results = new();

			foreach (var item in dictionary)
				results.Add(new() { Name = item.Key, Count = item.Value});

			return results;
		}
	}
}
