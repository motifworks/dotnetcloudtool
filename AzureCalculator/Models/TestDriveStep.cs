using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureCalculator.Models
{
    public class TestDriveStep
    {
        public int StepId { get; set; }
        public int TestDriveId { get; set; }
        public String StepName { get; set; }
        public int SequenceNumber { get; set; }
        public int ExpectedTime { get; set; }
        public String Statement { get; set; }
        public String StepDescription { get; set; }
        public String Remark { get; set; }
        public int Percentage { get; set; }

        public override string ToString()
        {
            return string.Format("TestDriveStep[{0}, {1}, {2}]", StepId, StepName, Statement);
        }
    }
}