using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor
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
