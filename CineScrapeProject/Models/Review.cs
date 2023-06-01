using System.Text.Json.Serialization;

namespace CineScrapeProject.wwwroot.Models
{
	public class Review
	{
		private string _urlImage;
		private string _name;
		private string _message;
		private string _fullReview;
		private DateTime _date;

		[JsonPropertyName("url_image")]
		public string UrlImage { get => _urlImage; set => _urlImage = value; }

		[JsonPropertyName("name")]
		public string Name { get => _name; set => _name = value; }

		[JsonPropertyName("message")]
		public string Message { get => _message; set => _message = value; }

		[JsonPropertyName("full_review")]
		public string FullReview { get => _fullReview; set => _fullReview = value; }

		[JsonPropertyName("date")]
		public DateTime Date { get => _date; set => _date = value; }
	}
}
