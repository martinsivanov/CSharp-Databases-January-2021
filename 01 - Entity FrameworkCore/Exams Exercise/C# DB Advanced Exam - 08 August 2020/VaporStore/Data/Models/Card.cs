using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {

        //•	Id – integer, Primary Key
        public int Id { get; set; }
        //•	Number – text, which consists of 4 pairs of 4 digits, separated by spaces(ex. “1234 5678 9012 3456”) (required)
        [Required]
        public string Number { get; set; }
        //•	Cvc – text, which consists of 3 digits(ex. “123”) (required)
        [Required]
        public string Cvc { get; set; }
        //•	Type – enumeration of type CardType, with possible values(“Debit”, “Credit”) (required)
        [Required]
        public CardType Type { get; set; }
        //•	UserId – integer, foreign key(required)
        public int UserId { get; set; }
        //•	User – the card’s user(required)
        [Required]
        public User User { get; set; }
        //•	Purchases – collection of type Purchase
        public ICollection<Purchase> Purchases { get; set; }

    }
}
