using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelTranscript.Models.Helper
{
    public class AudioWithMultiDocs
    {
        public int? AudioId { get; set; }
        public string Patients { get; set; }
        public string MRNs { get; set; }
        public string DOBs { get; set; }
        public string ModalityNames { get; set; }
        public string DOSs { get; set; }
        public string FileNames { get; set; }
    }
}