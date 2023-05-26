using CineScrapeProject.wwwroot.Models;

namespace CineScrapeProject.Models
{
	public static class Utilities
	{
		public static readonly string PATH = "sample-data/movies.json";

		public static string NullCritic(this int? num) => num.HasValue ? num.Value.ToString() : "--";

		public static Dictionary<string, int> Mode(this IEnumerable<Platform> source)
		{
			Dictionary<string, int> result = new();

			foreach (Platform item in source)
			{
				string key = Platform.PlatformsNames[item.Name];

				if (result.TryGetValue(key, out int value))
				{
					result[key] = value + 1;
				}
				else
				{
					result.Add(key, 1);
				}
			}

			return result;
		}
	}
}
