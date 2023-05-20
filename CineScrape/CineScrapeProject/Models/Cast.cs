namespace CineScrapeProject.wwwroot.Models
{
	public class Cast
	{
		private string _urlPhoto;
		private string _name;
		private string _character;

		public string UrlPhoto { get => _urlPhoto; set => _urlPhoto = value; }
		public string Name { get => _name; set => _name = value; }
		public string Character { get => _character; set => _character = value; }
	}
}
