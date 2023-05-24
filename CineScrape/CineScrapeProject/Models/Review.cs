using System.Text.Json.Serialization;

namespace CineScrapeProject.wwwroot.Models
{
	public class Review
	{
		private string _urlImage;
		private string _name;
		private string _message;
		private string _fullReview;
		public string _date;
		private DateTime _newDate;

		[JsonPropertyName("url_image")]
		public string UrlImage { get => _urlImage; set => _urlImage = value; }

		[JsonPropertyName("name")]
		public string Name { get => _name; set => _name = value; }

		[JsonPropertyName("message")]
		public string Message { get => _message; set => _message = value; }

		[JsonPropertyName("full_review")]
		public string FullReview { get => _fullReview; set => _fullReview = value; }

		[JsonPropertyName("date")]
		public string Date { get => _date; set { _date = value; Console.WriteLine(_date); } }

		public DateTime NewDate { get => _newDate; set => _newDate = value; }

		public static DateTime DateParse(string value)
		{
			//Console.WriteLine(value);

			string[] values = value.Split('/');

			//foreach (string value2 in values)
			//{
			//	Console.WriteLine(value2);
			//}

			return new DateTime(int.Parse(values[2]), int.Parse(values[1]), int.Parse(values[0]));
		}
	}
}
