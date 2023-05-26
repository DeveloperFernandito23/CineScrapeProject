using CineScrapeProject.Models;
using CineScrapeProject.wwwroot.Models;
using System.Net.Http.Json;

namespace CineScrapeProject.Pages
{
	public partial class Index
	{
		private enum Filters { RateCritics, RateAudience, Platforms }

		private Dictionary<string, int> _stadistics = new();
		private Filters _filterOption = Filters.RateCritics;

		public List<Movie> MovieList { get; set; }
		public Dictionary<string, int> Stadistics { get => _stadistics; set => _stadistics = value; }
		private Filters FilterOption { get => _filterOption; set { _filterOption = value; MakeStadistics(); } }

		protected override async Task OnInitializedAsync() => MovieList = await Http.GetFromJsonAsync<List<Movie>>(Utilities.PATH);
		protected override void OnParametersSet() => MakeStadistics();

		private void MakeStadistics()
		{
			Dictionary<string, int> results = new();

			switch (FilterOption)
			{
				case Filters.RateCritics:
					results = Rate(FilterOption);
					break;
				case Filters.RateAudience:
					results = Rate(FilterOption);
					break;
				case Filters.Platforms:
					results = Platforms();
					break;
			}

			Stadistics = results;
		}

		private Dictionary<string, int> Rate(Filters filter)
		{
			Dictionary<string, int> results = new()
			{
				{ "No rate or less than or equal 25", 0 },
				{ "Rate 26 - 50", 0 },
				{ "Rate 51 - 75", 0 },
				{ "Rate 76 - 100", 0 },
			};

			MovieList.ForEach(movie =>
			{
				int? property = filter == Filters.RateCritics ? movie.RateCritic : movie.RateAudience;

				if (!property.HasValue || property.Value <= 25)
				{
					results["No rate or less than or equal 25"]++;
				}
				else
				{
					int value = property.Value;

					if (value <= 50)
					{
						results["Rate 26 - 50"]++;
					}
					else if (value <= 75)
					{
						results["Rate 51 - 75"]++;
					}
					else
					{
						results["Rate 76 - 100"]++;
					}
				}
			});

			return results;
		}
		private Dictionary<string, int> Platforms()
		{
			Dictionary<string, int> results = new();

			List<Platform> platforms = new();

			MovieList.ForEach(movie => movie.Platforms.ForEach(platform => platforms.Add(platform)));

			results = platforms.Mode();

			return results;
		}
	}
}