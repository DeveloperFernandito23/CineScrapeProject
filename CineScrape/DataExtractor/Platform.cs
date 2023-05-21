using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataExtractor
{
    public class Platform
    {
        private string _image;
        private string _name;

		[JsonPropertyName("image")]
        public string Image { get => _image; set => _image = value; }

		[JsonPropertyName("name")]
        public string Name { get => _name; set => _name = value; }
	}
}
