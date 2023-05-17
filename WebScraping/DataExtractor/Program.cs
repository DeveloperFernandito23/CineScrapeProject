using Microsoft.Playwright;

namespace DataExtractor
{
	public class Program
	{
		static List<string> urlList = new List<string>();

		public static async Task Main(string[] args)
		{
			await RottenTomatoes();
		}

		public static async Task RottenTomatoes()
		{
			Microsoft.Playwright.Program.Main(new[] { "install" });

			using var playwright = await Playwright.CreateAsync();
			await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
			var page = await browser.NewPageAsync();

			await GetUrls(page, "https://www.rottentomatoes.com/browse/movies_in_theaters/");
			await GetUrls(page, "https://www.rottentomatoes.com/browse/movies_at_home/?page=10");
			await GetUrls(page, "https://www.rottentomatoes.com/browse/movies_coming_soon/");

			await MovieDetails(page);
		}

		public static async Task MovieDetails(IPage page)
		{
			foreach (string url in urlList)
			{
				await page.GotoAsync($"https://www.rottentomatoes.com{url}");

				Movie movie = new Movie();

				await GetTitle(page, movie);

				await GetRate(page, movie);

			}

			Console.WriteLine(urlList.Count);
		}

		public static async Task GetTitle(IPage page, Movie movie)
		{
			IElementHandle titleContainer = await page.QuerySelectorAsync("score-board");

			var title = await titleContainer.EvaluateHandleAsync($"container => container.shadowRoot.querySelector('slot[name = \"title\"]').assignedNodes()[0].innerHTML");

			movie.Title = title.ToString();

			Console.Write(title);
		}

		public static async Task GetRate(IPage page, Movie movie)
		{
			IElementHandle rateMainContainer = await page.QuerySelectorAsync("overlay-base[data-scoredetailsmanager]");

			var rateShadowRoot = await rateMainContainer.EvaluateHandleAsync($"container => container.shadowRoot.querySelector(`slot`).assignedNodes()[0].shadowRoot");

			var rateCritic = await rateShadowRoot.EvaluateHandleAsync($"container => container.querySelector(`slot[name = \"critics\"]`).assignedNodes()[0].shadowRoot.querySelector(`score-icon-critic`).shadowRoot.querySelector(`span.percentage`).innerHTML");
			var rateAudience= await rateShadowRoot.EvaluateHandleAsync($"container => container.querySelector(`slot[name = \"audience\"]`).assignedNodes()[0].shadowRoot.querySelector(`score-icon-audience`).shadowRoot.querySelector(`span.percentage`).innerHTML");

			var rateCriticValue = rateCritic.ToString().Split('%')[0];
			var rateAudienceValue = rateAudience.ToString().Split('%')[0];

			movie.RateCritic = rateCriticValue != "--" ? int.Parse(rateCriticValue) : 0;
			movie.RateAudience = rateAudienceValue != "--" ? int.Parse(rateAudienceValue) : 0;

			Console.WriteLine($", Crítica: {movie.RateCritic}, Audiencia: {movie.RateAudience}");
		}

		public static async Task AcceptCookies(IPage page)
		{
			ILocator acceptCookies = page.Locator("#onetrust-accept-btn-handler");

			await acceptCookies.WaitForAsync(new() { Timeout = 3000 });

			await acceptCookies.ClickAsync();
		}

		public static async Task GetUrls(IPage page, string url)
		{
			await page.GotoAsync(url);

			if (urlList.Count == 0) await AcceptCookies(page);

			IReadOnlyList<IElementHandle> movieContainer = await page.QuerySelectorAllAsync(".js-tile-link");

			foreach (IElementHandle movieContainerItem in movieContainer)
			{
				string urlMovie = await movieContainerItem.GetAttributeAsync("href");

				if (urlMovie == null)
				{
					IElementHandle newUrl = await movieContainerItem.QuerySelectorAsync("[data-track = \"scores\"]");

					urlMovie = await newUrl.GetAttributeAsync("href");
				}

				urlList.Add(urlMovie);
			}
		}
	}
}