using CineScrapeProject.Models;
using CineScrapeProject.wwwroot.Models;
using System.Net.Http.Json;

namespace CineScrapeProject.Pages
{
	public partial class Movies
	{
		private enum See { Cinema, Home, ComingSoon }

		private static readonly DateTime DATENOW = DateTime.Now;

		private List<Movie> _allMovies = new();
		private List<Movie> _movieList = new();
		private List<List<Movie>> _filtersMovies = new();
		private string _searchvalue = "";
		private List<Slot> _genders = new();
		private See _seeOption = See.Cinema;

		public List<Movie> AllMovies { get => _allMovies; set => _allMovies = value; }
		public List<Movie> MovieList { get => _movieList; set => _movieList = value; }
		public List<List<Movie>> FiltersMovies { get => _filtersMovies; set => _filtersMovies = value; }
		public string SearchValue { get => _searchvalue; set => _searchvalue = value; }
		public List<Slot> Genders { get => _genders; set => _genders = value; }
		private See SeeOption { get => _seeOption; set { _seeOption = value; WhereSee(); } }

		protected override async Task OnInitializedAsync()
		{
			AllMovies = await Http.GetFromJsonAsync<List<Movie>>(Utilities.PATH);

			MovieList = AllMovies;

			WhereSee();

			Genders = AllMovies.GenderFilter();

		}

		private void WhereSee()
		{
			List<Movie> movies = new();

			switch (SeeOption)
			{
				case See.Cinema:
					movies = Cinema();
					break;
				case See.Home:
					movies = AtHome();
					break;
				case See.ComingSoon:
					movies = ComingSoon();
					break;
			}

			MovieList = movies;
		}

		private List<Movie> Cinema()
		{
			List<Movie> movies = new();

			AllMovies.ForEach(movie =>
			{
				string dateString = GetDate(movie);

				if (!(dateString != null && DateTime.Parse(dateString) >= DATENOW))
					if (movie.Platforms.Contains(new() { Name = "Cinema" })) movies.Add(movie);
			});

			return movies;
		}

		private List<Movie> AtHome()
		{
			List<Movie> movies = new();

			AllMovies.ForEach(movie =>
			{
				string dateString = GetDate(movie);

				if(!(dateString != null && DateTime.Parse(dateString) >= DATENOW))
				{
					bool found = false;

					for (int i = 0; !found && i < movie.Platforms.Count; i++)
					{
						if (movie.Platforms[i].Name != "Cinema")
						{
							movies.Add(movie);
							found = true;
						}
					}
				}
			});

			return movies;
		}

		private List<Movie> ComingSoon()
		{
			List<Movie> movies = new();

			AllMovies.ForEach(movie =>
			{
				string dateString = GetDate(movie);

				if (dateString != null)
				{
					DateTime date = DateTime.Parse(dateString);

					if (DATENOW < date)
					{
						movies.Add(movie);
					}
				}
			});

			return movies;
		}

		private string GetDate(Movie movie)
		{
			string result = null;

			if (movie.Characteristics.TryGetValue("Release Date (Theaters)", out string value))
			{
				result = value;
			}

			return result;
		}

		private List<Movie> SearchMovie(List<Movie> allMovies)
		{
			List<Movie> results = new();

			if (SearchValue != "")
			{
				allMovies.ForEach(movie =>
				{
					if (movie.Title.ToUpper().Contains(SearchValue.ToUpper())) results.Add(movie);
				});
			}
			else
			{
				results = MovieList;
			}

			return results;
		}
	}
}