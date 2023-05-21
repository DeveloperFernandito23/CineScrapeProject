using CineScrapeProject.wwwroot.Models;
using System.Net.Http.Json;

namespace CineScrapeProject.Pages
{
    public partial class Movies
    {
        private List<Movie> _movieList = new List<Movie>();

		public List<Movie> MovieList { get => _movieList; set => _movieList = value; }

		protected override async Task OnInitializedAsync() => MovieList = await Http.GetFromJsonAsync<List<Movie>>("sample-data/movies.json");
    }
}