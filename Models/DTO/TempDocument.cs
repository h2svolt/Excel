using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelTranscript.Models.DTO
{
    public class Temp_DocumentList
    {
        public decimal TotalLines { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalDoc { get; set; }
        public string Clinic { get; set; }
        public string Provider { get; set; }

        public decimal GTotalLines { get; set; }
        public decimal GTotalAmount { get; set; }
        public int GTotalDoc { get; set; }

        public List<stp_DocumentReport_Result> Doclst;
    }
}