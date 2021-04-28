using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ImportProjectDto
    {
        //•	Name - text with length [2, 40] (required)
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }
        //•	OpenDate - date and time (required)
        [Required]
        public string OpenDate { get; set; }
        //•	DueDate - date and time (required)
        public string DueDate { get; set; }
        [XmlArray("Tasks")]
        public ImportTaskDto[] Tasks { get; set; }


    }
    [XmlType("Task")]
    public class ImportTaskDto
    {
        //•	Name - text with length[2, 40] (required)
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }
        //•	OpenDate - date and time (required)
        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }
        //•	DueDate - date and time (required)
        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }
        [Required]
        [Range(0,3)]
        [XmlElement("ExecutionType")]
        public int ExecutionType { get; set; }
        [Required]
        [Range(0,4)]
        [XmlElement("LabelType")]
        public int LabelType { get; set; }
    }
    //<Project>
    //<Name>S</Name>
    //<OpenDate>25/01/2018</OpenDate>
    //<DueDate>16/08/2019</DueDate>
    //<Tasks>
    //  <Task>
    //    <Name>Australian</Name>
    //    <OpenDate>19/08/2018</OpenDate>
    //    <DueDate>13/07/2019</DueDate>
    //    <ExecutionType>2</ExecutionType>
    //    <LabelType>0</LabelType>
    //  </Task>

}
