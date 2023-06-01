using CineScrapeProject.Models;
using CineScrapeProject.wwwroot.Models;
using Microsoft.AspNetCore.Components;
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
		private List<List<string>> _totalFilters = new();
		private List<Movie> _moviesOn = new();

			 
		public List<Movie> AllMovies { get => _allMovies; set => _allMovies = value; }
		public List<Movie> MovieList { get => _movieList; set => _movieList = value; }
		public List<List<Movie>> FiltersMovies { get => _filtersMovies; set => _filtersMovies = value; }
		public string SearchValue { get => _searchvalue; set => _searchvalue = value; }
		public List<Slot> Genders { get => _genders; set => _genders = value; }
		private See SeeOption { get => _seeOption; set { _seeOption = value; WhereSee(); } }
		public List<List<string>> TotalFilters { get => _totalFilters; set => _totalFilters = value; }
		public List<Movie> MoviesOn { get => _moviesOn; set => _moviesOn = value; }

		private void RestartFilters()
		{
			TotalFilters = new()
			{
				new()
			};

			MovieList = MoviesOn;
		}

		private void MakeFilters() //LO DE LOS FILTROS ESTABA PENSADO PARA PODER FILTRARLOS POR GÉNERO, RATE, ETC... LO QUE PASA ES QUE NO NOS HA DADO TIEMPO, PERO ESTÁ HECHO PARA FUTURAS APLIACIONES, POR ESO ES UNA LISTA Y SE COJE LA POSICIÓN 0 YA QUE LAS DEMÁS POSICIONES IBAN A SER LOS DEMÁS FILTROS :(((
		{
			List<Movie> movies = new();

			MoviesOn.ForEach(movie =>
			{
				if (movie.Characteristics.TryGetValue("Genre", out string value))
				{
					List<string> genders = new();

					bool add = true;

					for (int i = 0; add && i < TotalFilters[0].Count; i++)
					{
						if (!value.Contains(TotalFilters[0][i])) add = false;
					}

					if (add) movies.Add(movie);
				}
			});

			if (TotalFilters.SelectMany(x => x).Count() > 0)
			{
				MovieList = movies;
			}
			else
			{
				WhereSee();
			}
		}

		private void FilterGender(string value, bool add)
		{
			if (add)
			{
				if (!TotalFilters[0].Contains(value)) TotalFilters[0].Add(value);
			}
			else
			{
				TotalFilters[0].Remove(value);
			}

			MakeFilters();
		}

		protected override async Task OnInitializedAsync()
		{
			AllMovies = await Http.GetFromJsonAsync<List<Movie>>(Utilities.PATH);

			MovieList = AllMovies;

			WhereSee();

			Genders = AllMovies.GenderFilter();
		}
		
		protected override async Task OnParametersSetAsync() => RestartFilters();

		private void WhereSee()
		{
			switch (SeeOption)
			{
				case See.Cinema:
					MoviesOn = Cinema();
					break;
				case See.Home:
					MoviesOn = AtHome();
					break;
				case See.ComingSoon:
					MoviesOn = ComingSoon();
					break;
			}

			MovieList = MoviesOn;
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