using CineScrapeProject.Models;
using CineScrapeProject.wwwroot.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CineScrapeProject.Pages
{
	public partial class MovieDetails
	{
		private enum Order { Latest, Oldest }

		private const int LIMIT = 10;
		private const string CAROUSEL_HTML_ID = "carousel";

		private List<Movie> _movies = new();
		private Movie _movie = new();
		private List<Review> _reviews = new();
		private string _searchName;
		private string _searchMessage;
		private List<List<Review>> _paginationReviews = new();
		private int _page = 1;
		private int _count = 0;
		private Order _orderOption = Order.Latest;

		[Parameter]
		public string MovieId { get; set; }

		public Movie Movie { get => _movie; set => _movie = value; }
		public List<Movie> Movies { get => _movies; set => _movies = value; }
		public List<Review> Reviews { get => _reviews; set => _reviews = value; }
		public string SearchName { get => _searchName; set { _searchName = value; GetReviews(value, "name"); } }
		public string SearchMessage { get => _searchMessage; set { _searchMessage = value; GetReviews(value, "message"); } }

		public List<List<Review>> PaginationReviews { get => _paginationReviews; set => _paginationReviews = value; }
		public int Page { get => _page; set => _page = value > 0 && value <= PaginationReviews.Count ? value : 1; }
		public int MaxPage { get; set; }
		private Order OrderOption { get => _orderOption; set { _orderOption = value; RestartReviews(); SearchName = ""; SearchMessage = ""; } }


		protected override async Task OnInitializedAsync() => Movies = await Http.GetFromJsonAsync<List<Movie>>(Utilities.PATH);
		protected override async Task OnParametersSetAsync()
		{
			Movie = GetMovie(Movies, MovieId);

			RestartReviews();

			MaxPage = PaginationReviews.Count;
		}

		private void MakePagination(List<Review> allReviews)
		{
			_paginationReviews.Clear();

			allReviews = OrderByDate(allReviews);

			Reviews = allReviews;

			List<Review> reviewsPaginated = new List<Review>();

			int limitPosition = LIMIT - 1;
			int totalCount = allReviews.Count;

			for (int i = 0; i < totalCount; i++)
			{
				reviewsPaginated.Add(allReviews[i]);

				if (i == limitPosition || i == totalCount - 1)
				{
					PaginationReviews.Add(reviewsPaginated);
					limitPosition += LIMIT;
					reviewsPaginated = new();
				}
			}
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
		private void RestartReviews()
		{
			Reviews = Movie.Reviews;

			MakePagination(Reviews);
		}
		private List<Review> OrderByDate(List<Review> reviews)
		{
			List<Review> orderReviews = reviews.OrderByDescending(item => item.Date).ToList();

			if (OrderOption == Order.Oldest) orderReviews.Reverse();

			return orderReviews;
		}
		private List<Review> CheckOption(string value, string option)
		{
			List<Review> reviews = new();

			Page = 1;

			if (option == "name")
			{
				SearchMessage = "";
				Movie.Reviews.ForEach(review =>
				{
					if (review.Name.ToUpper().Contains(value.ToUpper()))
					{
						reviews.Add(review);
					}
				});
			}
			else
			{
				SearchName = "";
				Movie.Reviews.ForEach(review =>
				{
					if (review.Message.ToUpper().Contains(value.ToUpper()))
					{
						reviews.Add(review);
					}
				});
			}

			return reviews;
		}
		private void GetReviews(string value, string option)
		{
			if (value != "")
			{
				List<Review> reviews = CheckOption(value, option);

				Reviews = reviews;
				MaxPage = (int)Math.Ceiling((double)reviews.Count / 10);
			}
			else
			{
				Reviews = Movie.Reviews;
				MaxPage = (int)Math.Ceiling((double)Movie.Reviews.Count / 10);
			}
		}
		private List<Review> Pagination(List<Review> allReviews) => OrderByDate(allReviews.Count > LIMIT ? PaginationReviews[Page - 1] : allReviews);
	}
}