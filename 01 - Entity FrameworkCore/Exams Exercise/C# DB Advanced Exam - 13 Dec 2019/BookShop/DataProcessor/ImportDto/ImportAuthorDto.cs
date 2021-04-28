using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorDto
    {
        //•	FirstName - text with length [3, 30]. (required)
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string FirstName { get; set; }
        //•	LastName - text with length [3, 30]. (required)
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string LastName { get; set; }
        //•	Phone - text. Consists only of three groups (separated by '-'), the first two consist of three digits and the last one - of 4 digits. (required)
        [Required]
        [RegularExpression(@"^[0-9]{3}\-[0-9]{3}\-[0-9]{4}$")]
        public string Phone { get; set; }
        //•	Email - text (required). Validate it! There is attribute for this job.
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public BookDto[] Books { get; set; }

    }
    public class BookDto 
    {
        [JsonProperty("Id")]
        public int? BookId { get; set; }
    }
  //  "FirstName": "K",
  //  "LastName": "Tribbeck",
  //  "Phone": "808-944-5051",
  //  "Email": "btribbeck0@last.fm",
  //  "Books": [
  //    {
  //      "Id": 79
  //    },
  //    {
  //  "Id": 40
  //    }
  //  ]
  //},

}
