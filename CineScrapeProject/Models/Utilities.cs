using CineScrapeProject.wwwroot.Models;

namespace CineScrapeProject.Models
{
	public static class Utilities
	{
		private enum Months { Jan, Feb, Mar, Apr, May, Jun, Jul, Ago, Sep, Oct, Nov, Dec }


		public static readonly string PATH = "./sample-data/movies.json";

		public static string NullCritic(this int? num) => num.HasValue ? $"{num.Value}%" : "--";

		public static string PrintDate(this DateTime date)
		{
			string[] dateString = date.ToString().Split(' ')[0].Split('/');

			Months month = (Months)(int.Parse(dateString[1]) - 1);

			int day = int.Parse(dateString[0]);

			int year = int.Parse(dateString[2]);

			return $"{month} {day}, {year}";
		}

		public static List<Slot> PlatformFilter(this IEnumerable<Platform> source)
		{
			Dictionary<string, int> dictionary = new();

			foreach (Platform item in source)
			{
				string key = item.Name;

				dictionary = CheckKey(dictionary, key);
			}

			List<Slot> results = new();

			foreach (var item in dictionary)
				results.Add(new() { Name = item.Key, Count = item.Value });

			return results;
		}
		public static List<Slot> GenderFilter(this IEnumerable<Movie> source)
		{
			Dictionary<string, int> dictionary = new();

			foreach (Movie movie in source)
			{
				if (movie.Characteristics.TryGetValue("Genre", out string value))
				{
					string[] values = value.Split(',');

					foreach (string item in values)
						dictionary = CheckKey(dictionary, item.Trim());
				}
			}

			List<Slot> results = new();

			foreach (var item in dictionary)
				results.Add(new() { Name = item.Key, Count = item.Value });

			return results;
		}

		private static Dictionary<string, int> CheckKey(Dictionary<string, int> dictionary, string key)
		{
			if (dictionary.TryGetValue(key, out int value))
			{
				dictionary[key] = ++value;
			}
			else
			{
				dictionary.Add(key, 1);
			}

			return dictionary;
		}
	}
}
