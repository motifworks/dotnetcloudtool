using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCalculator.TestDrives
{
    public interface ITestDrive
    {
        String ExecuteStep(User user, TestDrive drive, TestDriveStep step);

        String GetMailContent(User user, TestDrive drive);

        String GetMailSubject(User user, TestDrive drive);

        String GetAccessibilityDetails(User user, TestDrive drive);
    }
}
