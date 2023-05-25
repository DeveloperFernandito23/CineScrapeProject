using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataExtractor
{
	public class Review
	{
		private enum Months { Jan, Feb, Mar, Apr, May, Jun, Jul, Ago, Sep, Oct, Nov, Dec }

		private string _urlImage;
		private string _name;
		private string _message;
		private string _fullReview;
		private DateTime _date;

		[JsonPropertyName("url_image")]
		public string UrlImage { get => _urlImage; set => _urlImage = value; }

		[JsonPropertyName("name")]
		public string Name { get => _name; set => _name = value; }

		[JsonPropertyName("message")]
		public string Message { get => _message; set => _message = value; }

		[JsonPropertyName("full_review")]
		public string FullReview { get => _fullReview; set => _fullReview = value; }

		[JsonPropertyName("date")]
		public DateTime Date { get => _date; set => _date = value; }

		public string GetDate(string date)
		{
			string[] newDate = date.Split(' ');

			int month = (int)Enum.Parse(typeof(Months), newDate[0]) + 1;

			int year = int.Parse(newDate[2]);

			int day = int.Parse(newDate[1].TrimEnd(','));

			DateTime dateTime = new DateTime(year, month, day);

			return dateTime.ToString().Split(' ')[0];
		}
	}
}
