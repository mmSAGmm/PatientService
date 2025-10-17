namespace Db.DataAccess.Options
{
	public class MongoOption
	{
		public string? ConnectionString { get; set; }
		public string DatabaseName { get; set; } = "patients";
		public string CollectionName { get; set; } = "tbPatients";
	}
}
