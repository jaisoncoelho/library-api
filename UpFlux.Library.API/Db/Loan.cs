using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UpFlux.Library.API.Db
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string User { get; set; }
        public DateTime Borrowed { get; set; }
        public DateTime? Returned { get; set; }
    }
}
