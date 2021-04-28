using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class ImportCustomerTricketsDto
    {
        //•	FirstName – text with length [3, 20] (required)
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        //•	LastName – text with length [3, 20] (required)
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [XmlElement("LastName")]
        public string LastName { get; set; }
        //•	Age – integer in the range [12, 110] (required)
        [Range(12, 110)]
        [XmlElement("Age")]
        public int Age { get; set; }
        //•	Balance - decimal (non-negative, minimum value: 0.01) (required)
        [XmlElement("Balance")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }
        [XmlArray("Tickets")]
        public TicketDto[] Tickets { get; set; }
    }
    [XmlType("Ticket")]
    public class TicketDto
    {
        [XmlElement("ProjectionId")]
        public int ProjectionId { get; set; }
        [XmlElement("Price")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
    //<Customer>
    //<FirstName>Randi</FirstName>
    //<LastName>Ferraraccio</LastName>
    //<Age>20</Age>
    //<Balance>59.44</Balance>
    //<Tickets>
    //  <Ticket>
    //    <ProjectionId>1</ProjectionId>
    //    <Price>7</Price>
    //  </Ticket>

}
