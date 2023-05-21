using CineScrapeProject.wwwroot.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CineScrapeProject.Pages
{
	public partial class MovieDetails
	{
		private List<Movie> _movies = new List<Movie>();
		private Movie _movie = new Movie();
		private List<Review> _reviews = new List<Review>();
		private string _searchValue;

		[Parameter]
		public string MovieId { get; set; }

		public Movie Movie { get => _movie; set => _movie = value; }
		public List<Movie> Movies { get => _movies; set => _movies = value; }
		public List<Review> Reviews { get => _reviews; set => _reviews = value; }
		public string SearchValue { get => _searchValue; set { _searchValue = value; GetReviews(value); } }

		protected override async Task OnInitializedAsync() => Movies = await Http.GetFromJsonAsync<List<Movie>>("sample-data/movies.json");
		protected override async Task OnParametersSetAsync()
		{
			Movie = GetMovie(Movies, MovieId);

			Reviews = Movie.Reviews;
		}

		private Movie GetMovie(List<Movie> movies, string movieId)
		{
			Movie movie = new Movie();
			bool found = false;

			for (int i = 0; !found && i < movies.Count; i++)
			{
				if (movies[i].Id == int.Parse(movieId))
				{
					movie = movies[i];
					found = true;
				}
			}

			return movie;
		}
		private async Task GetReviews(string value)
		{
			List<Review> reviews = new List<Review>();

			if (value != "")
			{
				foreach (Review review in Movie.Reviews)
				{
					if (review.Name.Contains(value))
					{
						reviews.Add(review);
					}
				}
			}
			else
			{
				reviews = Movie.Reviews;
			}

			Reviews = reviews;
		}
	}
}