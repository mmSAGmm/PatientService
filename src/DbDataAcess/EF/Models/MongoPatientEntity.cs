using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DataAccess.EF.Models
{
    public class MongoPatientEntity
    {
        public string Id { get; set; } = string.Empty;
        public string Json { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }
}
