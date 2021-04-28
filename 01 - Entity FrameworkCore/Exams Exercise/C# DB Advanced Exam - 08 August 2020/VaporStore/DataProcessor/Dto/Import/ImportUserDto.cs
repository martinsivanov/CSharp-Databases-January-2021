using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUserDto
    {
        //•	FullName – text, which has two words, consisting of Latin letters. Both start with an upper letter and are followed by lower letters. The two words are separated by a single space (ex. "John Smith") (required)
        [Required]
        [RegularExpression(@"^([A-Z]{1}[a-z]+)\s([A-Z]{1}[a-z]+)$")]

        public string FullName { get; set; }
        //•	Username – text with length [3, 20] (required)
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }
        //•	Email – text (required)
        [Required]
        public string Email { get; set; }
        //•	Age – integer in the range [3, 103] (required)
        [Required]
        [Range(3, 103)]
        public int Age { get; set; }
        public ImportCardDto[] Cards { get; set; }
    }
    public class ImportCardDto
    {
        //•	Number – text, which consists of 4 pairs of 4 digits, separated by spaces (ex. “1234 5678 9012 3456”) (required)
        [Required]
        [RegularExpression(@"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$")]
        public string Number { get; set; }
        //•	Cvc – text, which consists of 3 digits (ex. “123”) (required)
        [Required]
        [RegularExpression(@"^[0-9]{3}$")]
        public string CVC { get; set; }
        //•	Type – enumeration of type CardType, with possible values (“Debit”, “Credit”) (required)
        [Required]
        public string Type { get; set; }
    }
    //  {
    //  "FullName": "Lorrie Silbert",
    //  "Username": "lsilbert",
    //  "Email": "lsilbert@yahoo.com",
    //  "Age": 33,
    //  "Cards": [
    //    {
    //      "Number": "1833 5024 0553 6211",
    //      "CVC": "903",
    //      "Type": "Debit"
    //    },
    //    {
    //  "Number": "5625 0434 5999 6254",
    //      "CVC": "570",
    //      "Type": "Credit"
    //    },
    //    {
    //  "Number": "4902 6975 5076 5316",
    //      "CVC": "091",
    //      "Type": "Debit"
    //    }
    //  ]
    //},

}
