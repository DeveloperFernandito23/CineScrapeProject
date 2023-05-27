using BlazorBootstrap;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.PolarAreaChart;
using ChartJs.Blazor.ScatterChart;
using ChartJs.Blazor.Util;
using CineScrapeProject.Models;
using CineScrapeProject.wwwroot.Models;
using System.Drawing;
using System.Net.Http.Json;
using System.Reflection.Emit;

namespace CineScrapeProject.Pages
{
	public partial class Index
	{
		private enum Filters { RateCritics, RateAudience, Platforms, Runtime, Genders }

		private readonly List<TimeSpan> RUNTIMEFILTERS = new()
		{
			new(1, 30, 0),
			new(2, 0, 0),
			new(2, 30, 0),
			new(3, 0, 0)
		};

		List<ToastMessage> messages = new();

		private PieConfig _pieConfig = new();
		private List<string> _casasolors = new List<string>();
		private PieConfig _doughConfig = new();
		private PolarAreaConfig _polarConfig = new();
		private BarConfig _barConfig = new();
		private List<Slot> _stadistics = new();
		private Filters _filterOption = Filters.RateCritics;
		private List<Slot> _genders = new();

		public List<Movie> MovieList { get; set; }
		public List<Slot> Stadistics { get => _stadistics; set => _stadistics = value; }
		private Filters FilterOption { get => _filterOption; set { _filterOption = value; MakeStadistics(); } }

		public List<Slot> Genders { get => _genders; set => _genders = value; }
		public PieConfig PieConfig { get => _pieConfig; set => _pieConfig = value; }
		public PieConfig DoughConfig { get => _doughConfig; set => _doughConfig = value; }
		public PolarAreaConfig PolarConfig { get => _polarConfig; set => _polarConfig = value; }
		public BarConfig BarConfig { get => _barConfig; set => _barConfig = value; }

		protected override async Task OnInitializedAsync()
		{
			MovieList = await Http.GetFromJsonAsync<List<Movie>>(Utilities.PATH);

			Genders = MovieList.GenderFilter();

			ShowMessage(ToastType.Success);

			ColorUtil.RandomColorString();
		}


		private async Task CreateCharts()
		{
			PieConfig = new PieConfig
			{
				Options = new PieOptions
				{
					Responsive = true,
					Title = new OptionsTitle
					{
						Display = false,
						Text = "ChartJs.Blazor Pie Chart"
					}
				}
			};

			DoughConfig = new PieConfig
			{
				Options = new PieOptions
				{
					Responsive = true,
					CutoutPercentage = 50,
					Title = new OptionsTitle
					{
						Display = true,
						Text = "ChartJs.Blazor Pie Chart"
					}
				}
			};

			BarConfig = new BarConfig
			{
				Options = new BarOptions
				{
					Responsive = true,
					Title = new OptionsTitle
					{
						Display = true,
						Text = "ChartJs.Blazor Pie Chart"
					},
					Legend = new Legend
					{
						Display = true,
					}

				}
			};

			PolarConfig = new PolarAreaConfig
			{
				Options = new PolarAreaOptions
				{
					Responsive = true,
					Title = new OptionsTitle
					{
						Display = true,
						Text = "alksjals"
					}
				}
			};
		}
		protected override async Task OnParametersSetAsync()
		{
			await MakeStadistics();
		}

		private async Task FillCharts(Filters filterSelected)
		{

			switch (FilterOption)
			{
				case Filters.Platforms:
					FillDoughChart();
					break;
				case Filters.Runtime:
					FillBarChart();
					break;
				case Filters.Genders:
					FillPolarChart();
					break;
				default:
					FillPieChart();
					break;
			}


		}

		
		protected override void OnInitialized()
		{
			CreateCharts();

			for (int i = 0; i < 30; i++)
			{
				string color = ColorUtil.RandomColorString();
				_casasolors.Add(color);
			}
		}
		private async Task MakeStadistics()
		{
			List<Slot> results = new();

			switch (FilterOption)
			{
				case Filters.RateCritics:
					results = RateStats(FilterOption);
					break;
				case Filters.RateAudience:
					results = RateStats(FilterOption);
					break;
				case Filters.Platforms:
					results = PlatformsStats();
					break;
				case Filters.Runtime:
					results = RuntimeStats();
					break;
				case Filters.Genders:
					results = Genders;
					break;
			}

			Stadistics = results;


			await FillCharts(FilterOption);
		}

		private List<Slot> RateStats(Filters filter)
		{
			List<Slot> results = new()
			{
				new() { Name = "No rate or less than or equal 25%"},
				new() { Name = "Rate 26% - 50%"},
				new() { Name = "Rate 51% - 75%"},
				new() { Name = "Rate 76% - 100%"},
			};

			MovieList.ForEach(movie =>
			{
				int? property = filter == Filters.RateCritics ? movie.RateCritic : movie.RateAudience;

				if (!property.HasValue || property.Value <= 25)
				{
					results[0].Count++;
				}
				else
				{
					int value = property.Value;

					if (value <= 50)
					{
						results[1].Count++;
					}
					else if (value <= 75)
					{
						results[2].Count++;
					}
					else
					{
						results[3].Count++;
					}
				}
			});

			return results;
		}
		private List<Slot> PlatformsStats()
		{
			List<Platform> platforms = new();

			MovieList.ForEach(movie => movie.Platforms.ForEach(platform => platforms.Add(platform)));

			return platforms.PlatformFilter();
		}
		private List<Slot> RuntimeStats()
		{
			List<Slot> results = new()
			{
				new() { Name =  "Runtime less than or equal 1h 30m"},
				new() { Name =  "Runtime 1h 30m - 2h" },
				new() { Name =  "Runtime 2h - 2h 30m" },
				new() { Name =  "Runtime 2h 30m - 3h" },
				new() { Name =  "Runtime greater than 3h" }
			};

			List<TimeSpan> times = GetRuntimes();

			times.ForEach(time =>
			{
				if (time <= RUNTIMEFILTERS[0])
				{
					results[0].Count++;
				}
				else if (time <= RUNTIMEFILTERS[1])
				{
					results[1].Count++;
				}
				else if (time <= RUNTIMEFILTERS[2])
				{
					results[2].Count++;
				}
				else if (time <= RUNTIMEFILTERS[3])
				{
					results[3].Count++;
				}
				else
				{
					results[4].Count++;
				}
			});

			return results;
		}

		private List<TimeSpan> GetRuntimes()
		{
			List<TimeSpan> runtimes = new();

			MovieList.ForEach(movie =>
			{
				if (movie.Characteristics.TryGetValue("Runtime", out string value))
				{
					string[] values = value.Split(' ');

					int hours = 0, minutes = 0;

					for (int i = 0; i < values.Length; i++)
					{
						if (values[i].Contains('h')) hours = int.Parse(values[i].TrimEnd('h'));
						if (values[i].Contains('m')) minutes = int.Parse(values[i].TrimEnd('m'));
					}

					TimeSpan timeSpan = new TimeSpan(hours, minutes, 0);

					runtimes.Add(timeSpan);
				}
			});

			return runtimes;
		}
		private void ShowMessage(ToastType toastType) => messages.Add(CreateToastMessage(toastType));

		private ToastMessage CreateToastMessage(ToastType toastType)
		=> new ToastMessage
		{
			Type = toastType,
			Title = "Blazor Bootstrap",
			HelpText = $"{DateTime.Now}",
			Message = $"Hello, world! This is a toast message. DateTime: {DateTime.Now}",
		};

		private void FillPieChart()
		{
			PieConfig.Data.Labels.Clear();
			PieConfig.Data.Datasets.Clear();

			foreach (Slot slot in Stadistics)
			{
				PieConfig.Data.Labels.Add(slot.Name);
			}

			PieDataset<int> dataset = new PieDataset<int>()
			{

				BackgroundColor =  _casasolors.Take(Stadistics.Count).ToArray()
			};
			foreach (Slot slot in Stadistics)
			{
				dataset.Add(slot.Count);
			}
			PieConfig.Data.Datasets.Add(dataset);
		}

		private void FillDoughChart()
		{
			DoughConfig.Data.Labels.Clear();
			DoughConfig.Data.Datasets.Clear();

			foreach (Slot slot in Stadistics)
			{
				DoughConfig.Data.Labels.Add(slot.Name);
			}

			PieDataset<int> dataset = new PieDataset<int>()
			{

				BackgroundColor = _casasolors.Take(Stadistics.Count).ToArray()
			};
			foreach (Slot slot in Stadistics)
			{
				dataset.Add(slot.Count);
			}
			DoughConfig.Data.Datasets.Add(dataset);
		}
		private void FillBarChart()
		{
			BarConfig.Data.Labels.Clear();
			BarConfig.Data.Datasets.Clear();

			foreach (Slot slot in Stadistics)
			{
				BarConfig.Data.Labels.Add(slot.Name);
			}

			BarDataset<int> dataset = new BarDataset<int>()
			{
				BackgroundColor = _casasolors.Take(Stadistics.Count).ToArray(),
				Label = "Runtime of the movies"
			};
			foreach (Slot slot in Stadistics)	
			{
				dataset.Add(slot.Count);
			}
			BarConfig.Data.Datasets.Add(dataset);
		}
		private void FillPolarChart()
		{
			PolarConfig.Data.Labels.Clear();
			PolarConfig.Data.Datasets.Clear();

			foreach (Slot slot in Stadistics)
			{
				PolarConfig.Data.Labels.Add(slot.Name);
			}

			PolarAreaDataset<int> dataset = new PolarAreaDataset<int>()
			{

				BackgroundColor = _casasolors.Take(Stadistics.Count).ToArray()
			};
			foreach (Slot slot in Stadistics)
			{
				dataset.Add(slot.Count);
			}
			PolarConfig.Data.Datasets.Add(dataset);
		}
	}
}