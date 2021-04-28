using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.Data.Models
{
    public class AuthorBook
    {

        //•	AuthorId - integer, Primary Key, Foreign key(required)
        public int AuthorId { get; set; }
        //•	Author -  Author
        public Author Author { get; set; }
        //•	BookId - integer, Primary Key, Foreign key(required)
        public int BookId { get; set; }
        //•	Book - Book
        public Book Book { get; set; }

    }
}
