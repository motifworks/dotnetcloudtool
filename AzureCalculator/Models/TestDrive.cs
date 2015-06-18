using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureCalculator.Models
{
    public class TestDrive
    {
        public int TestDriveId { get; set; }

        public String TestDriveName { get; set; }

        public String TestDriveIcon { get; set; }

        public String ToolTip { get; set; }

        public String TestDriveSiteURL { get; set; }

        public int ISVId { get; set; }

        public String Purpose { get; set; }

        public int ExpiryTime { get; set; }

        public String TestDriveDescription { get; set; }

        public String IntroductionHTML { get; set; }

        public String LabInfo1 { get; set; }

        public String LabInfo2 { get; set; }

        public String SubmitCaption { get; set; }

        public String ProgressDetails { get; set; }

        public String MailDetails { get; set; }

        public String Controller { get; set; }

        public int ExpectedTime { get; set; }

        public String TestDriveTitle { get; set; }

        public String LogFolder { get; set; }

        public String DeleteCommand { get; set; }

        public String MailSubject { get; set; }

        public String MailContent { get; set; }

        public override string ToString()
        {
            return string.Format("TestDrive[{0}, {1}]", TestDriveId, TestDriveName);
        }
    }
}