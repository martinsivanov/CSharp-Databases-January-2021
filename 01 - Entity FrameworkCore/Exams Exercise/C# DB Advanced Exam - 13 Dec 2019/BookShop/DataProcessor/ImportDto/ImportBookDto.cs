using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class ImportBookDto
    {
        //•	Name - text with length [3, 30]. (required)
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        public string Name { get; set; }
        //•	Genre - enumeration of type Genre, with possible values (Biography = 1, Business = 2, Science = 3) (required)
        [Required]
        [XmlElement("Genre")]
        [Range(1,3)]
        public int Genre { get; set; }
        //•	Price - decimal in range between 0.01 and max value of the decimal
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        [XmlElement("Price")]
        public decimal Price { get; set; }
        //•	Pages – integer in range between 50 and 5000
        [Range(50,5000)]
        [XmlElement("Pages")]
        public int Pages { get; set; }
        //•	PublishedOn - date and time (required)
        [Required]
        [XmlElement("PublishedOn")]
        public string PublishedOn { get; set; }
    }
    //<Book>
    //  <Name>Hairy Torchwood</Name>
    //  <Genre>3</Genre>
    //  <Price>41.99</Price>
    //  <Pages>3013</Pages>
    //  <PublishedOn>01/13/2013</PublishedOn>
    //</Book>

}
