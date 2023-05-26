using CineScrapeProject.Models;
using CineScrapeProject.Shared;
using CineScrapeProject.wwwroot.Models;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace CineScrapeProject.Pages
{
    public partial class Movies
    {
        private List<Movie> _movieList = new();
        private string _searchvalue = "";
		private List<Slot> _genders = new();

		public List<Movie> MovieList { get => _movieList; set => _movieList = value; }
		public string SearchValue { get => _searchvalue; set => _searchvalue = value; }
		public List<Slot> Genders { get => _genders; set => _genders = value; }

        protected override async Task OnInitializedAsync()
        {
            MovieList = await Http.GetFromJsonAsync<List<Movie>>(Utilities.PATH);

            Genders = MovieList.GenderFilter();
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