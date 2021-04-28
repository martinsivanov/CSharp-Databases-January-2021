﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Projection")]
    public class ImportProjectionDto
    {
        [XmlElement("MovieId")]
        public int MovieId { get; set; }
        [XmlElement("HallId")]
        public int HallId { get; set; }
        [XmlElement("DateTime")]
        public string DateTime { get; set; }
    }
    //  <Projection>
    //  <MovieId>38</MovieId>
    //  <HallId>4</HallId>
    //  <DateTime>2019-04-27 13:33:20</DateTime>
    //</Projection>

}
