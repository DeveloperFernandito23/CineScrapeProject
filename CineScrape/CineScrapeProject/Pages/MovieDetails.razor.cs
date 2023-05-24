using CineScrapeProject.wwwroot.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CineScrapeProject.Pages
{
	public partial class MovieDetails
	{
		private const int LIMIT = 10;
		private enum Order { Latest, Oldest}

		private List<Movie> _movies = new();
		private Movie _movie = new();
		private List<Review> _reviews = new();
		private string _searchName;
		private string _searchMessage;
		private List<List<Review>> _paginationReviews = new();
		private int _page = 1;
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
		private Order OrderOption { get => _orderOption; set => _orderOption = value; }

		protected override async Task OnInitializedAsync() => Movies = await Http.GetFromJsonAsync<List<Movie>>("sample-data/movies.json");
		protected override async Task OnParametersSetAsync()
		{
			Movie = await GetMovie(Movies, MovieId);

			Reviews = Movie.Reviews;

			await MakePagination(Reviews);

			MaxPage = PaginationReviews.Count;

			//Movie.Reviews.ForEach((review) => {
			//	review.NewDate = Review.DateParse(review.Date);
			//});
		}

		private async Task MakePagination(List<Review> allReviews)
		{
			List<Review> reviews = new List<Review>();

			int limitPosition = LIMIT - 1;
			int totalCount = allReviews.Count;

			for (int i = 0; i < totalCount; i++)
			{
				reviews.Add(allReviews[i]);

				if (i == limitPosition || i == totalCount - 1)
				{
					PaginationReviews.Add(reviews);
					limitPosition += LIMIT;
					reviews = new();
				}
			}
		}

		private	IEnumerable<Review> OrderByDate(List<Review> reviews)
		{
			List<Review> paginatedReviews = Pagination(reviews);

			paginatedReviews.ForEach(review =>
			{
				review.NewDate = Review.DateParse(review._date);
			});

			var e = paginatedReviews.OrderBy(item => item.NewDate);

			

			//foreach (Review review in e)
			//{
			//	Console.WriteLine(review.NewDate.ToString().Split(' ')[0]);
			//}

			return e;
		}

		//private List<Review> OrderDate(List<Review> reviews)
		//{
		//	return reviews.Sort((x, y) => GetDate(x).CompareTo(GetDate(y)));
		//}

		private async Task<Movie> GetMovie(List<Movie> movies, string movieId)
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

		private List<Review> Pagination(List<Review> allReviews) => allReviews.Count > LIMIT ? PaginationReviews[Page - 1] : allReviews;
	}
}