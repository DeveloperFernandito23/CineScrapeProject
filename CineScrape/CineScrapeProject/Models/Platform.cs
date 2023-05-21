using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineScrapeProject.wwwroot.Models
{
    public class Platform
    {
        private string _image;
        private string _name;

        public string Image { get => _image; set => _image = value; }
        public string Name { get => _name; set => _name = value; }
    }
}
