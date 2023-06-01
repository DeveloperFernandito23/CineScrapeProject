using System.Text.Json.Serialization;

namespace DataExtractor
{
	public class Cast
	{
		private string _urlPhoto;
		private string _name;
		private string _character;

		[JsonPropertyName("url_photo")]
		public string UrlPhoto { get => _urlPhoto; set => _urlPhoto = value; }

		[JsonPropertyName("name")]
		public string Name { get => _name; set => _name = value; }

		[JsonPropertyName("character")]
		public string Character { get => _character; set => _character = value; }
	}
}
