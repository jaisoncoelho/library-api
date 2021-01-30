using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UpFlux.Library.API.Profiles
{
    public class LibraryProfile : AutoMapper.Profile
    {
        public LibraryProfile()
        {
            CreateMap<Db.Book, Models.Book>();
            CreateMap<Models.Book, Db.Book>();
            CreateMap<Db.Loan, Models.Loan>();
            CreateMap<Models.Loan, Db.Loan>();
        }
    }
}
