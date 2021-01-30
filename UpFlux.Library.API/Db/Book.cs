using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UpFlux.Library.API.Db
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public ICollection<Loan> Loans { get; set; }

        [NotMapped]
        public bool IsAvailable { get; set; }
    }
}
