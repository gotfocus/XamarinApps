using SQLite;

namespace CaSI
{
	[Table("CaSIitem")]
	public class CaSIItem
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public string Name { get; set; }
		public bool Done { get; set; }
	}
}
