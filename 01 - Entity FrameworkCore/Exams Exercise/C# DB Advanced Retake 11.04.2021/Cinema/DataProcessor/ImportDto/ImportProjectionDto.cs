using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Projection")]
    public class ImportProjectionDto
    {
        //TODO: Maybe check if its negative?
        [XmlElement("MovieId")]
        [Required]
        public int MovieId { get; set; }
        [XmlElement("DateTime")]
        [Required]
        public string DateTime { get; set; }
    }
  //  <Projection>
  //  <MovieId>6</MovieId>
  //  <DateTime>2019-05-12 05:51:29</DateTime>
  //</Projection>
}
