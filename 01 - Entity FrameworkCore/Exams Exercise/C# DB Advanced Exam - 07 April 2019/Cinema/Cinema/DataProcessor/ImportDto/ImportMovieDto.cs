using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportMovieDto
    {
        //•	Title – text with length [3, 20] (required)
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [JsonProperty("Title")]
        public string Title { get; set; }
        //•	Genre – enumeration of type Genre, with possible values (Action, Drama, Comedy, Crime, Western, Romance, Documentary, Children, Animation, Musical) (required)
        [Required]
        [JsonProperty("Genre")]
        public string Genre { get; set; }
        //•	Duration – TimeSpan (required)
        [Required]
        [JsonProperty("Duration")]
        public string Duration { get; set; }
        //•	Rating – double in the range [1,10] (required)
        [Range(1,10)]
        [JsonProperty("Rating")]
        public double Rating { get; set; }
        //•	Director – text with length [3, 20] (required)
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [JsonProperty("Director")]
        public string Director { get; set; }
    }
//{
//    "Title": "Gui Si (Silk)",
//    "Genre": "Drama",
//    "Duration": "02:21:00",
//    "Rating": 9,
//    "Director": "Perl Swyne"
//  }
}
