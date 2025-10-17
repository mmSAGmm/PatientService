namespace Db.DataAccess.EF.Models
{
    public class Patient
    {
        public string Id { get; set; } = string.Empty;

        public string Json { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }
}
