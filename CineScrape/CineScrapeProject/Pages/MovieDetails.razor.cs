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
		private string _searchName;
		private string _searchDescription;

		[Parameter]
		public string MovieId { get; set; }

		public Movie Movie { get => _movie; set => _movie = value; }
		public List<Movie> Movies { get => _movies; set => _movies = value; }
		public List<Review> Reviews { get => _reviews; set => _reviews = value; }
		public string SearchName { get => _searchName; set { _searchName = value; GetReviews(value, "name"); } }
		public string SearchDescription { get => _searchDescription; set { _searchDescription = value; GetReviews(value, "message"); } }

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
		private void GetReviews(string value, string option)
		{
			if (value != "")
			{
				Reviews = CheckOption(value, option);
			}
			else
			{
				Reviews = Movie.Reviews;
			}
		}
		private List<Review> CheckOption(string value, string option)
		{
			List<Review> reviews = new List<Review>();

			foreach (Review review in Movie.Reviews)
			{
				if (option == "name")
				{
					if (review.Name.ToUpper().Contains(value.ToUpper()))
					{
						reviews.Add(review);
					}
				}
				else
				{
					if (review.Message.ToUpper().Contains(value.ToUpper()))
					{
						reviews.Add(review);
					}
				}
			}

			return reviews;
		}
	}
}