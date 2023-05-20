namespace CineScrapeProject.wwwroot.Models
{
	public class Review
	{
		private string _urlImage;
		private string _name;
		private string _message;
		private string _fullReview;
		private string _date;

		public string UrlImage { get => _urlImage; set => _urlImage = value; }
		public string Name { get => _name; set => _name = value; }
		public string Message { get => _message; set => _message = value; }
		public string FullReview { get => _fullReview; set => _fullReview = value; }
		public string Date { get => _date; set => _date = value; }
	}
}
