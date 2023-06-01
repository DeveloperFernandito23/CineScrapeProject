using System.Text.Json.Serialization;

namespace CineScrapeProject.wwwroot.Models
{
	public class Platform
	{
		private string _image;
		private string _name;
		private string _urlSite;

		[JsonPropertyName("image")]
		public string Image { get => _image; set => _image = value; }

		[JsonPropertyName("name")]
		public string Name { get => _name; set => _name = value; }

		[JsonPropertyName("urlSite")]
		public string UrlSite { get => _urlSite; set => _urlSite = value; }

		public override bool Equals(object obj)
		{
			Platform other = obj as Platform;
			bool result = false;

			if(other != null)
			{
				if (Name == other.Name)
				{
					result = true;
				}
			}

			return result;
		}
	}
}
