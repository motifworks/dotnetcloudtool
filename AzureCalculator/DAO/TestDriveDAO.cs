using AzureCalculator.Helper;
using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureCalculator.DAO
{
    public class TestDriveDAO
    {
        public static TestDrive Get(int testDriveId)
        {
            String query = "SELECT TestDriveTitle, IntroductionHTML, LabInfo1, LabInfo2, SubmitCaption, ProgressDetails, MailDetails, ISVId, TestDriveId, TestDriveName, TestDriveSiteURL, Purpose, TestDriveIcon, ToolTip, ExpiryTime, TestDriveDescription, Controller, LogFolder, DeleteCommand, MailSubject, MailContent FROM TestDrive WHERE TestDriveId = @TestDriveId";
            return DatabaseHelper.Get<TestDrive>(query, new { TestDriveId = testDriveId }).Single();
        }

        public static TestDrive GetByName(String testDriveName)
        {
            String query = "SELECT TestDriveTitle, IntroductionHTML, LabInfo1, LabInfo2, SubmitCaption, ProgressDetails, MailDetails, ISVId, TestDriveId, TestDriveName, TestDriveSiteURL, Purpose, TestDriveIcon, ToolTip, ExpiryTime, TestDriveDescription, Controller, LogFolder, DeleteCommand, MailSubject, MailContent FROM TestDrive WHERE UPPER(REPLACE(TestDriveName, ' ', '')) = UPPER(REPLACE(@TestDriveName, ' ', ''))";
            return DatabaseHelper.Get<TestDrive>(query, new { TestDriveName = testDriveName }).Single();
        }

        public static List<TestDriveStep> GetSteps(int testDriveId)
        {
            String query = "SELECT StepId, TestDriveId, StepName, SequenceNumber, ExpectedTime, Statement, StepDescription, Remark, Percentage FROM Step WHERE TestDriveId = @TestDriveId ORDER BY SequenceNumber";
            return DatabaseHelper.Get<TestDriveStep>(query, new { TestDriveId = testDriveId });
        }
    }
}