using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UpFlux.Library.API.Models
{
    public class Book
    {
        public Book()
        {
            IsAvailable = true;
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }
        public bool IsAvailable { get; set; }
    }
}
