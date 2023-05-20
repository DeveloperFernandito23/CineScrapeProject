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
    public partial class Movies
    {
        private static List<Movie> _movieList = new();
        public static List<Movie> MoviesList { get => _movieList; set => _movieList = value; }

        protected override async Task OnInitializedAsync()
        {
            MoviesList = await Http.GetFromJsonAsync<List<Movie>>("sample-data/movies.json");
        }
    }
}