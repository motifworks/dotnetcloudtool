using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureCalculator.Models
{
    public class User
    {
        public int UserId { get; set; }
        public String UserName { get; set; }
        public String CompanyName { get; set; }
        public String Email { get; set; }
        public String ContactNumber { get; set; }
        public String LoginPassword { get; set; }
        public String LoginId { get; set; }
        public String Status { get; set; }
        public String SiteName { get; set; }
        public DateTime? RequestedOn { get; set; }
        public DateTime? AvailableOn { get; set; }
        public DateTime? ExpireOn { get; set; }
        public DateTime? RemovedOn { get; set; }
        public int TestDriveId { get; set; }
    }
}