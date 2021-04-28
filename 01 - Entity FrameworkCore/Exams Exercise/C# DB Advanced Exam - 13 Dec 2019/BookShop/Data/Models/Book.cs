using BookShop.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.Data.Models
{
    public class Book
    {
        // •	Id - integer, Primary Key
        public int Id { get; set; }
        //•	Name - text with length[3, 30]. (required)
        [Required]
        //[MaxLength(30)]
        public string Name { get; set; }
        //•	Genre - enumeration of type Genre, with possible values(Biography = 1, Business = 2, Science = 3) (required)
        [Required]
        public Genre Genre { get; set; }
        //•	Price - decimal in range between 0.01 and max value of the decimal
        //[Range(typeof(decimal),"0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
        //•	Pages – integer in range between 50 and 5000
        //[Range(50,5000)]
        public int Pages { get; set; }
        //•	PublishedOn - date and time(required)
        [Required]
        public DateTime PublishedOn { get; set; }
        //•	AuthorsBooks - collection of type AuthorBook
        public ICollection<AuthorBook> AuthorsBooks { get; set; }

    }
}
