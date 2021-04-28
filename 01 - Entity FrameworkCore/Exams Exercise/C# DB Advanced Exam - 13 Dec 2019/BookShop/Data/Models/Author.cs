using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.Data.Models
{
    public class Author
    {
        // •Id - integer, Primary Key
        public int Id { get; set; }
        //•	FirstName - text with length[3, 30]. (required)
        [Required]
        //[MaxLength(30)]
        public string FirstName { get; set; }
        //•	LastName - text with length[3, 30]. (required)
        [Required]
        //[MaxLength(30)]
        public string LastName { get; set; }
        //•	Email - text(required). Validate it! There is attribute for this job.
        [Required]
        //[EmailAddress]
        public string Email { get; set; }
        //•	Phone - text.Consists only of three groups(separated by '-'), the first two consist of three digits and the last one - of 4 digits. (required)
        [Required]
        public string Phone { get; set; }
        //•	AuthorsBooks - collection of type AuthorBook
        public ICollection<AuthorBook> AuthorsBooks { get; set; }

    }
}
