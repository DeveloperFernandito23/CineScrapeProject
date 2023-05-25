using Microsoft.Playwright;
using System.Text.Json;


namespace DataExtractor
{
	public class Program
	{
		public const int NUMPAGE = 5;
		public const string MAINURL = "https://www.rottentomatoes.com";
		public const string PATH = "../../../../CineScrapeProject/wwwroot/sample-data/movies.json";
		public const string DEFAULTREVIEWURL = "https://images.fandango.com/cms/assets/5b6ff500-1663-11ec-ae31-05a670d2d590--rtactordefault.png";
		public const string DEFAULTCASTURL = "https://images.fandango.com/cms/assets/b0cefeb0-b6a8-11ed-81d8-51a487a38835--poster-default-thumbnail.jpg";
		public const string DEFAULTREVIEWURLNEW = "https://pbs.twimg.com/profile_images/1249207089987301376/IM529qEB_400x400.jpg";
		public const string DEFAULTCASTURLNEW = "https://img.redbull.com/images/c_crop,x_1676,y_0,h_1310,w_1048/c_fill,w_860,h_1075/q_auto:low,f_auto/redbullcom/2023/2/22/qpctvo5nspffj1vgy3a1/red-bull-click-cabecera";

		public static List<string> _urlList = new();
		public static List<Movie> _movies = new();

		public static async Task Main() => await RottenTomatoesAsync();
		public static async Task RottenTomatoesAsync()
		{
			Microsoft.Playwright.Program.Main(new[] { "install" });

			using var playwright = await Playwright.CreateAsync();
			await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
			var page = await browser.NewPageAsync();

			await GetUrlsAsync(page, $"{MAINURL}/browse/movies_in_theaters/");
			await GetUrlsAsync(page, $"{MAINURL}/browse/movies_at_home/?page={NUMPAGE}");
			await GetUrlsAsync(page, $"{MAINURL}/browse/movies_coming_soon/");

			await MovieDetailsAsync(page);
		}
		public static async Task GetUrlsAsync(IPage page, string url)
		{
			await page.GotoAsync(url);

			if (_urlList.Count == 0) await AcceptCookiesRTAsync(page);

			IReadOnlyList<IElementHandle> movieContainer = await page.QuerySelectorAllAsync(".js-tile-link");

			foreach (IElementHandle movieContainerItem in movieContainer)
			{
				string urlMovie = await movieContainerItem.GetAttributeAsync("href");

				if (urlMovie == null)
				{
					IElementHandle newUrl = await movieContainerItem.QuerySelectorAsync("[data-track = \"scores\"]");

					urlMovie = await newUrl.GetAttributeAsync("href");
				}

				_urlList.Add(urlMovie);
			}
		}
		public static async Task MovieDetailsAsync(IPage page)
		{
			int id = 1;

			foreach (string url in _urlList)
			{
				Movie movie = new();

				movie.Id = id++;

				string newUrl = $"{MAINURL}{url}";

				//string newUrl = "https://www.rottentomatoes.com/m/the_super_mario_bros_movie";

				await page.GotoAsync(newUrl);

				await GetTitleAsync(page, movie);

				await GetPosterAsync(page, movie);

				await GetRateAsync(page, movie);

				await GetPlatformsAsync(page, movie);

				await GetInfoAsync(page, movie);

				await GetPhotosAsync(page, movie);

				await GetCastAsync(page, movie);

				await GetReviewsAsync(page, movie, newUrl);

				await GetTrailerAsync(page, movie);

				_movies.Add(movie);
			}

			string moviesJSON = JsonSerializer.Serialize<List<Movie>>(_movies);

			File.WriteAllText(PATH, moviesJSON);
		}
		public static async Task GetPosterAsync(IPage page, Movie movie)
		{
			IElementHandle posterContainer = await page.QuerySelectorAsync("tile-dynamic.thumbnail");

			var poster = await posterContainer.EvaluateHandleAsync("container => container.shadowRoot.querySelector('slot[name = \"image\"]').assignedNodes()[0].getAttribute('src')");

			movie.PosterURL = poster.ToString();
			Console.WriteLine(poster.ToString());
		}
		public static async Task GetTitleAsync(IPage page, Movie movie)
		{
			IElementHandle titleContainer = await page.QuerySelectorAsync("score-board");

			var title = await titleContainer.EvaluateHandleAsync("container => container.shadowRoot.querySelector('slot[name = \"title\"]').assignedNodes()[0].innerHTML");

			movie.Title = title.ToString();

			Console.Write(title);
		}
		public static async Task GetInfoAsync(IPage page, Movie movie)
		{
			IElementHandle infoContainer = await page.QuerySelectorAsync("#movie-info .panel-body");

			await GetDescriptionAsync(movie, infoContainer);

			await GetCharacteristicsAsync(movie, infoContainer);
		}
		public static async Task GetDescriptionAsync(Movie movie, IElementHandle infoContainer)
		{
			IElementHandle descriptionContainer = await infoContainer.QuerySelectorAsync("drawer-more");

			string description = await descriptionContainer.EvaluateAsync<string>($"container => container.shadowRoot.querySelector('slot[name = \"content\"]').assignedNodes()[0].innerHTML");

			description = description.Trim('\n', ' ');

			movie.Description = description;
		}
		public static async Task GetCharacteristicsAsync(Movie movie, IElementHandle infoContainer)
		{
			IReadOnlyList<IElementHandle> characteristicsContainer = await infoContainer.QuerySelectorAllAsync("#info li p");

			foreach (var item in characteristicsContainer)
			{
				var label = await item.QuerySelectorAsync("b");
				var value = await item.QuerySelectorAsync("span");

				movie.Characteristics.Add(await label.InnerTextAsync(), await value.InnerTextAsync());
			}
		}
		public static async Task GetCastAsync(IPage page, Movie movie)
		{
			IReadOnlyList<IElementHandle> castsContainer = await page.QuerySelectorAllAsync(".cast-and-crew-item ");

			foreach (var castData in castsContainer)
			{
				Cast cast = new Cast();

				var image = await castData.QuerySelectorAsync("img");

				string urlPhoto = await image.GetAttributeAsync("src");
				cast.UrlPhoto = CheckCastImage(urlPhoto);

				string name = await image.GetAttributeAsync("alt");
				cast.Name = name;

				var characterContainer = await castData.QuerySelectorAsync("div > p");
				string character = await characterContainer.InnerTextAsync();

				cast.Character = CheckCharacter(character.Trim('\n', ' '));

				Console.WriteLine();
				movie.Casts.Add(cast);
			}

		}
		public static async Task GetPhotosAsync(IPage page, Movie movie)
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
		public static async Task GetRateAsync(IPage page, Movie movie)
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
		public static async Task GetPlatformsAsync(IPage page, Movie movie)
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
						Platform platform = new Platform();

						var platformContainer = await platformsContainer.EvaluateHandleAsync($"{selector}[{i}].shadowRoot.querySelector('slot[name = \"bubble\"]').assignedNodes()[0].shadowRoot.querySelector('affiliate-icon').shadowRoot.querySelector('img')");

						string image = await platformContainer.EvaluateAsync<string>("container => container.getAttribute('src')");
						platform.Image = $"{MAINURL}{image}";

						string name = await platformContainer.EvaluateAsync<string>("container => container.getAttribute('alt')");
						platform.Name = name;

						movie.Platforms.Add(platform);
					}
				}
			}
		}
		public static async Task GetReviewsAsync(IPage page, Movie movie, string url)
		{
			await page.GotoAsync($"{url}/reviews");

			IElementHandle reviewsContainer = await page.QuerySelectorAsync("#reviews .review_table");

			IReadOnlyList<IElementHandle> buttons = await page.QuerySelectorAllAsync("rt-button:not(.hide)");

			await FillReviewAsync(reviewsContainer, movie);

			if (buttons.Count != 0)
			{
				await buttons[0].ClickAsync();

				await FillReviewAsync(reviewsContainer, movie);
			}
		}
		public static async Task FillReviewAsync(IElementHandle reviewsContainer, Movie movie)
		{
			Thread.Sleep(1000);

			IReadOnlyList<IElementHandle> reviews = await reviewsContainer.QuerySelectorAllAsync(".review-row");

			foreach (IElementHandle reviewData in reviews)
			{
				Review review = new Review();

				var image = await reviewData.QuerySelectorAsync(".critic-picture");
				string urlImage = await image.GetAttributeAsync("src");
				review.UrlImage = CheckReviewImage(urlImage);

				var name = await reviewData.QuerySelectorAsync(".display-name");
				string nameText = await name.InnerHTMLAsync();
				review.Name = nameText.Trim('\n', ' ');

				var message = await reviewData.QuerySelectorAsync(".review-text");
				review.Message = await message.InnerHTMLAsync();

				var fullReviewAndDate = await reviewData.QuerySelectorAsync(".original-score-and-url");

				var fullReview = await fullReviewAndDate.QuerySelectorAsync("a");
				var fullReviewData = await fullReview?.GetAttributeAsync("href");
				review.FullReview = fullReviewData ?? "https://developerfernandito23.github.io/Pokedex/";

				var date = await fullReviewAndDate.QuerySelectorAsync("span");
				string dateParsed = await date.InnerHTMLAsync();
				review.Date = DateTime.Parse(dateParsed);

				movie.Reviews.Add(review);
			}

			Console.WriteLine("gg");
		}

		
		public static async Task GetTrailerAsync(IPage page, Movie movie)
		{
			await page.GotoAsync("https://www.youtube.com/");

			if (_movies.Count == 0) await AcceptCookiesYTAsync(page);

			IElementHandle search = await page.QuerySelectorAsync("input#search");
			await search.TypeAsync($"TRAILER OFFICIAL {movie.Title}");

			var button = await page.QuerySelectorAsync("button#search-icon-legacy");
			await button.ClickAsync();

			Thread.Sleep(3000);

			IReadOnlyList<IElementHandle> videos = await page.QuerySelectorAllAsync("#contents.ytd-item-section-renderer ytd-video-renderer");

			var video = await videos.First().QuerySelectorAsync("a#thumbnail");

			var link = await video.GetAttributeAsync("href");

			string newLink = CheckURL(link);

			movie.Trailer = $"https://www.youtube.com/embed/{newLink}";
		}
		public static async Task AcceptCookiesYTAsync(IPage page)
		{
			ILocator acceptCookies = page.Locator("button[aria-label = 'Aceptar el uso de cookies y otros datos para las finalidades descritas']");

			await acceptCookies.WaitForAsync(new() { Timeout = 4000 });

			await acceptCookies.ClickAsync();

			Thread.Sleep(3000);
		}
		public static async Task AcceptCookiesRTAsync(IPage page)
		{
			ILocator acceptCookies = page.Locator("#onetrust-accept-btn-handler");

			await acceptCookies.WaitForAsync(new() { Timeout = 3000 });

			await acceptCookies.ClickAsync();
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
		public static string CheckURL(string url) => url.Split('?')[1].Substring(2, 11);
		public static string CheckReviewImage(string urlImage) => urlImage == DEFAULTREVIEWURL ? DEFAULTREVIEWURLNEW : urlImage;
		public static string CheckCastImage(string urlImage) => urlImage == DEFAULTCASTURL ? DEFAULTCASTURLNEW : urlImage;
	}
}