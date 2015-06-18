using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureCalculator.Models
{
    public class ISV
    {
        public int ISVId { get; set; }
        
        public String ISVName { get; set; }
        
        public String ISVSiteURL { get; set; }
        
        public String ISVIcon { get; set; }

        public String ISVDescription { get; set; }

        public String HeaderBackground { get; set; }

        public String ISVContact { get; set; }

        public String ISVTitle { get; set; }

        public int SequenceNumber { get; set; }
    }
}