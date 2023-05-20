using Microsoft.Playwright;
using System.Text.Json;

namespace DataExtractor
{
	public class Program
	{
		public static List<string> urlList = new();
		public const string MAINURL = "https://www.rottentomatoes.com";
		public static List<Movie> movies = new List<Movie>();

		public static async Task Main(string[] args)
		{
			await RottenTomatoes();
		}

		public static async Task RottenTomatoes()
		{
			Microsoft.Playwright.Program.Main(new[] { "install" });

			using var playwright = await Playwright.CreateAsync();
			await using var browser = await playwright.Firefox.LaunchAsync(new() { Headless = false });
			var page = await browser.NewPageAsync();

			await GetUrls(page, $"{MAINURL}/browse/movies_in_theaters/");
			await GetUrls(page, $"{MAINURL}/browse/movies_at_home/?page=1");
			await GetUrls(page, $"{MAINURL}/browse/movies_coming_soon/");

			await MovieDetails(page);
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
		public static async Task MovieDetails(IPage page)
		{
			foreach (string url in urlList)
			{
				string newUrl = $"{MAINURL}{url}";

				//string newUrl = "https://www.rottentomatoes.com/m/the_super_mario_bros_movie";

				await page.GotoAsync(newUrl);

				Movie movie = new();

				await GetTitle(page, movie);

				await GetRate(page, movie);

				await GetInfo(page, movie);

				//await GetTrailer(page, movie);

				await GetPlatforms(page, movie);

				await GetPhotos(page, movie);

				await GetCast(page, movie);

				await GetPoster(page, movie);

				await GetReviews(page, movie, newUrl);

				movies.Add(movie);
			}

			string e = JsonSerializer.Serialize<List<Movie>>(movies);

			File.WriteAllText("../../../../data/movies.json", e);

			Console.WriteLine(urlList.Count);
		}
		public static async Task GetPoster(IPage page, Movie movie)
		{
			IElementHandle posterContainer = await page.QuerySelectorAsync("tile-dynamic.thumbnail");

			var poster = await posterContainer.EvaluateHandleAsync("container => container.shadowRoot.querySelector('slot[name = \"image\"]').assignedNodes()[0].getAttribute('src')");

			movie.PosterURL = poster.ToString();
			Console.WriteLine(poster.ToString());
		}
		public static async Task GetTitle(IPage page, Movie movie)
		{
			IElementHandle titleContainer = await page.QuerySelectorAsync("score-board");

			var title = await titleContainer.EvaluateHandleAsync("container => container.shadowRoot.querySelector('slot[name = \"title\"]').assignedNodes()[0].innerHTML");

			movie.Title = title.ToString();

			Console.Write(title);
		}
		public static async Task GetInfo(IPage page, Movie movie)
		{
			IElementHandle infoContainer = await page.QuerySelectorAsync("#movie-info .panel-body");

			await GetDescription(movie, infoContainer);

			await GetCharacteristics(movie, infoContainer);
		}
		public static async Task GetDescription(Movie movie, IElementHandle infoContainer)
		{
			IElementHandle descriptionContainer = await infoContainer.QuerySelectorAsync("drawer-more");

			string description = await descriptionContainer.EvaluateAsync<string>($"container => container.shadowRoot.querySelector('slot[name = \"content\"]').assignedNodes()[0].innerHTML");

			description = description.Trim('\n', ' ');

			movie.Description = description;
		}
		public static async Task GetCharacteristics(Movie movie, IElementHandle infoContainer)
		{
			IReadOnlyList<IElementHandle> characteristicsContainer = await infoContainer.QuerySelectorAllAsync("#info li p");

			foreach (var item in characteristicsContainer)
			{
				var label = await item.QuerySelectorAsync("b");
				var value = await item.QuerySelectorAsync("span");

				movie.Characteristics.Add(await label.InnerTextAsync(), await value.InnerTextAsync());
			}
		}
		public static async Task GetCast(IPage page, Movie movie)
		{
			IReadOnlyList<IElementHandle> castsContainer = await page.QuerySelectorAllAsync(".cast-and-crew-item ");

			foreach (var castData in castsContainer)
			{
				Cast cast = new Cast();

				var image = await castData.QuerySelectorAsync("img");
				cast.UrlPhoto = await image.GetAttributeAsync("src");
				cast.Name= await image.GetAttributeAsync("alt");

				var characterContainer = await castData.QuerySelectorAsync("div > p");
				string character = await characterContainer.InnerTextAsync();

				cast.Character = CheckCharacter(character.Trim('\n', ' '));

				Console.WriteLine();
				movie.Casts.Add(cast);
			}
			
		}
		public static string CheckCharacter(string character)
		{
			string result = "";

			for (int i = 0; i < character.Length; i++)
			{
				char res = character[i];

				if (res == '\n')
				{
					int j = i;

					while (res == '\n' || res == ' ')
					{
						res = character[j];
						result += "";
						j++;
					}

					result += " ";
					i = --j;
				}

				result += character[i];
			}

			return result;
		}
		public static async Task GetPhotos(IPage page, Movie movie)
		{
			IElementHandle carrousel = await page.QuerySelectorAsync("[role = \"listbox\"]");

			IReadOnlyList<IElementHandle> photos = await carrousel.QuerySelectorAllAsync(".slick-slide");

			foreach (var item in photos)
			{
				IElementHandle image = await item.QuerySelectorAsync("img");

				string urlImage = await image.GetAttributeAsync("src");

				movie.Photos.Add(urlImage);
			}
		}
		public static async Task GetRate(IPage page, Movie movie)
		{
			IElementHandle rateMainContainer = await page.QuerySelectorAsync("overlay-base[data-scoredetailsmanager]");

			var rateShadowRoot = await rateMainContainer.EvaluateHandleAsync("container => container.shadowRoot.querySelector(`slot`).assignedNodes()[0].shadowRoot");

			var rateCritic = await rateShadowRoot.EvaluateHandleAsync("container => container.querySelector(`slot[name = \"critics\"]`).assignedNodes()[0].shadowRoot.querySelector(`score-icon-critic`).shadowRoot.querySelector(`span.percentage`).innerHTML");
			var rateAudience = await rateShadowRoot.EvaluateHandleAsync("container => container.querySelector(`slot[name = \"audience\"]`).assignedNodes()[0].shadowRoot.querySelector(`score-icon-audience`).shadowRoot.querySelector(`span.percentage`).innerHTML");

			var rateCriticValue = rateCritic.ToString().Split('%')[0];
			var rateAudienceValue = rateAudience.ToString().Split('%')[0];

			movie.RateCritic = rateCriticValue != "--" ? int.Parse(rateCriticValue) : null;
			movie.RateAudience = rateAudienceValue != "--" ? int.Parse(rateAudienceValue) : null;

			Console.WriteLine($", Crítica: {movie.RateCritic}, Audiencia: {movie.RateAudience}");
		}
		public static async Task GetPlatforms(IPage page, Movie movie)
		{
			IElementHandle platformsContainer = await page.QuerySelectorAsync("bubbles-overflow-container");

			if (platformsContainer != null)
			{
				string selector = "container => container.shadowRoot.querySelectorAll('slot')[0].assignedNodes()";

				var length = await platformsContainer.EvaluateHandleAsync($"{selector}.length");

				for (int i = 0; i < int.Parse(length.ToString()); i++)
				{
					if (i % 2 != 0)
					{
						var platform = await platformsContainer.EvaluateHandleAsync($"{selector}[{i}].shadowRoot.querySelector('slot[name = \"bubble\"]').assignedNodes()[0].shadowRoot.querySelector('affiliate-icon').shadowRoot.querySelector('img').getAttribute('alt')");

						movie.Platforms.Add(platform.ToString());
					}
				}
			}
		}
		public static async Task GetReviews(IPage page, Movie movie, string url)
		{
			await page.GotoAsync($"{url}/reviews");

			IElementHandle reviewsContainer = await page.QuerySelectorAsync("#reviews .review_table");

			IReadOnlyList<IElementHandle> buttons = await page.QuerySelectorAllAsync("rt-button:not(.hide)");

			await FillReview(reviewsContainer, movie);

			if (buttons.Count != 0)
			{
				await buttons[0].ClickAsync();

				await FillReview(reviewsContainer, movie);
			}

		}
		public static async Task FillReview(IElementHandle reviewsContainer, Movie movie)
		{
			IReadOnlyList<IElementHandle> reviews = await reviewsContainer.QuerySelectorAllAsync(".review-row");

			foreach (IElementHandle reviewData in reviews)
			{
				Review review = new Review();

				var image = await reviewData.QuerySelectorAsync(".critic-picture");
				review.UrlImage = await image.GetAttributeAsync("src");

				var name = await reviewData.QuerySelectorAsync(".display-name");
				string nameText = await name.InnerHTMLAsync();
				review.Name = nameText.Trim('\n', ' ');

				var message = await reviewData.QuerySelectorAsync(".review-text");
				review.Message = await message.InnerHTMLAsync();

				var fullReviewAndDate = await reviewData.QuerySelectorAsync(".original-score-and-url");

				var fullReview = await fullReviewAndDate.QuerySelectorAsync("a");
				review.FullReview = await fullReview.GetAttributeAsync("href");

				var date = await fullReviewAndDate.QuerySelectorAsync("span");
				review.Date = await date.InnerHTMLAsync();

				movie.Reviews.Add(review);
			}
		}
		public static async Task GetTrailer(IPage page, Movie movie)
		{
			IElementHandle buttonContainer = await page.QuerySelectorAsync("#hero-image");

			await buttonContainer.EvaluateAsync("container => container.shadowRoot.querySelector('slot[name = \"tile\"]').assignedNodes()[0].shadowRoot.querySelector('slot').assignedNodes()[1].shadowRoot.querySelector('slot[name = \"imageAction\"]').assignedNodes()[0].click()");

			IElementHandle videoContainer = await page.QuerySelectorAsync("overlay-base.visible");

			var link = await videoContainer.EvaluateHandleAsync("container => container.shadowRoot.querySelector('slot[name = \"content\"]').assignedNodes()[0].shadowRoot.querySelector('slot[name = \"content\"]').assignedNodes()[1].querySelector('video').getAttribute('src')");
		}
		public static async Task AcceptCookies(IPage page)
		{
			ILocator acceptCookies = page.Locator("#onetrust-accept-btn-handler");

			await acceptCookies.WaitForAsync(new() { Timeout = 3000 });

			await acceptCookies.ClickAsync();
		}
	}
}