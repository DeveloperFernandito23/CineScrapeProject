using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using CineScrapeProject;
using CineScrapeProject.Shared;
using CineScrapeProject.wwwroot.Models;

namespace CineScrapeProject.Pages
{
    public partial class Index
    {
        private static List<Movie> _movies = new();

        public static List<Movie> Movies { get => _movies; set => _movies = value; }
        protected override async Task OnInitializedAsync()
        {
            Movies = await Http.GetFromJsonAsync<List<Movie>>("sample-data/movies.json");
        }
    }
}