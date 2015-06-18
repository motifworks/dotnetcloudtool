using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureCalculator.Models
{
    public class StatusType
    {
        public static String STATUS_REQUESTED = "R";
        public static String STATUS_PROVISIONING = "P";
        public static String STATUS_COMPLETED = "C";
        public static String STATUS_ERROR = "E";
        public static String STATUS_EXPIRED = "X";
    }

    public class Status
    {
        public int StatusId { get; set; }

        public int UserId { get; set; }

        public int StepId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public String StepStatus { get; set; }

        public int TimeTaken { get; set; }

        public String Remark { get; set; }

        public int SequenceNumber { get; set; }
    }
}