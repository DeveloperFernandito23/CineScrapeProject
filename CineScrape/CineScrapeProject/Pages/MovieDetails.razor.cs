using CineScrapeProject.wwwroot.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CineScrapeProject.Pages
{
	public partial class MovieDetails
	{
		private List<Movie> _movies = new List<Movie>();
		private Movie _movie = new Movie();

		[Parameter]
		public string MovieName { get; set; }

		public Movie Movie { get => _movie; set => _movie = value; }
		public List<Movie> Movies { get => _movies; set => _movies = value; }

		protected override async Task OnInitializedAsync() => Movies = await Http.GetFromJsonAsync<List<Movie>>("sample-data/movies.json");
		protected override async Task OnParametersSetAsync() => Movie = GetMovie(Movies, MovieName);

		private Movie GetMovie(List<Movie> movies, string movieName)
		{
			Movie movie = new Movie();
			bool found = false;

			for (int i = 0; !found && i < movies.Count; i++)
			{
				if (movies[i].Title == movieName)
				{
					movie = movies[i];
					found = true;
				}
			}

			return movie;
		}
	}
}
