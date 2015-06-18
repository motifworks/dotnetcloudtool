using AzureCalculator.Helper;
using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureCalculator.DAO
{
    public class ISVDAO
    {
        public static ISV Get(int isvId)
        {
            String query = "SELECT ISVTitle, ISVId, ISVName, ISVSiteURL, ISVIcon, ISVDescription, HeaderBackground, ISVContact, ISVSequence AS SequenceNumber FROM ISV WHERE ISVId = @ISVId";
            return DatabaseHelper.Get<ISV>(query, new { ISVId = isvId }).SingleOrDefault();
        }

        public static List<ISV> GetAll()
        {
            String query = "SELECT ISVTitle, ISVId, ISVName, ISVSiteURL, ISVIcon, ISVDescription, HeaderBackground, ISVContact, ISVSequence AS SequenceNumber FROM ISV ORDER BY ISVSequence";
            return DatabaseHelper.Get<ISV>(query, new { });
        }

        public static ISV GetByName(String isvName)
        {
            String query = "SELECT ISVTitle, ISVId, ISVName, ISVSiteURL, ISVIcon, ISVDescription FROM ISV WHERE UPPER(REPLACE(ISVName, ' ', '')) = UPPER(REPLACE(@isvName, ' ', ''))";
            return DatabaseHelper.Get<ISV>(query, new { ISVName = isvName }).SingleOrDefault();
        }

        public static List<TestDrive> GetAvailableTestDrives(int isvId)
        {
            String query = "SELECT TestDriveTitle, IntroductionHTML, LabInfo1, LabInfo2, SubmitCaption, ProgressDetails, MailDetails, ISVId, TestDriveId, TestDriveName, TestDriveSiteURL, Purpose, TestDriveIcon, ToolTip, ExpiryTime, TestDriveDescription, Controller, LogFolder, DeleteCommand, MailSubject, MailContent FROM TestDrive WHERE ISVId = @ISVId";
            return DatabaseHelper.Get<TestDrive>(query, new { ISVId = isvId });
        }
    }
}