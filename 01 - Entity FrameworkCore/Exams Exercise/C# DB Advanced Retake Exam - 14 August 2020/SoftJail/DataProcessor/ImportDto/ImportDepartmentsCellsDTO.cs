using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentsCellsDTO
    {
        [MinLength(3)]
        [MaxLength(25)]
        [Required]
        public string Name { get; set; }

        public CellDTO[] Cells { get; set; }
    }
    public class CellDTO
    {
        [Range(1,1000)]
        [Required]
        public int CellNumber { get; set; }

        [Required]
        public bool HasWindow { get; set; }
    }

    //[
  //{
  //  "Name": "",
  //  "Cells": [
  //    {
  //      "CellNumber": 101,
  //      "HasWindow": true
  //    },
  //    {
  //  "CellNumber": 102,
  //      "HasWindow": false
  //    }
  //  ]

}
