namespace CineScrapeProject.Models
{
	public class Slot
	{
		private string _name;
		private int _count;

		public string Name { get => _name; set => _name = value; }
		public int Count { get => _count; set => _count = value; }
	}
}