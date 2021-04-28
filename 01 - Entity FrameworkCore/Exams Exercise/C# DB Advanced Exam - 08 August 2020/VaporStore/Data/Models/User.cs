using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        //•	Id – integer, Primary Key
        public int Id { get; set; }
        //•	Username – text with length[3, 20] (required)
        [Required]
        public string Username { get; set; }
        //•	FullName – text, which has two words, consisting of Latin letters.Both start with an upper letter and are followed by lower letters.The two words are separated by a single space (ex. "John Smith") (required)
        [Required]
        public string FullName { get; set; }
        //•	Email – text(required)
        [Required]
        public string Email { get; set; }
        //•	Age – integer in the range[3, 103] (required)
        public int Age { get; set; }
        //•	Cards – collection of type Card
        public ICollection<Card> Cards { get; set; }

    }
}
