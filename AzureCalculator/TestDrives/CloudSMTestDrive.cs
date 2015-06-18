using AzureCalculator.Helper;
using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace AzureCalculator.TestDrives
{
    public class CloudSMTestDrive : ITestDrive
    {
        public String ExecuteStep(User user, TestDrive drive, TestDriveStep step)
        {
            try
            {
                PSScriptHelper.RunScript(StringHelper.ReplaceParameter(step.Statement, user), StringHelper.CreateQualifiedFileName(drive.LogFolder, StringHelper.RemoveSpecialCharacters(user.SiteName) + "-" + StringHelper.RemoveSpecialCharacters(step.StepName) + ".log"));
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }

        public String GetMailSubject(User user, TestDrive drive)
        {
            return String.IsNullOrEmpty(drive.MailSubject) ? "Ostrato - CloudSM on Azure Test Drive" : drive.MailSubject;
        }

        public String GetAccessibilityDetails(User user, TestDrive drive)
        {
            String accessDetails = "";
            accessDetails += "<BR>Your CloudSM&trade; is ready. Please click <a target='_blank' href='https://" + user.SiteName + ".cloudapp.net'>here</a> to login. Please find the site details.";
            accessDetails += GetConnectionDetails(user, drive);
            accessDetails += "<BR><B>Please Note site will be available for 2 hours from now</B>";
            return accessDetails;
        }

        public String GetMailContent(User user, TestDrive drive)
        {
            String html = "Dear " + user.UserName + ",";

            html += "<BR/><BR/>Thank you for trying Ostrato CloudSM&trade; on Azure Test Drive from Motifworks.";
            html += "<BR/><BR/>Your CloudSM&trade; site is ready. Please find the details below to login: ";
            html += GetConnectionDetails(user, drive);
            html += "<BR/><BR/><p style='font-size:11px'>Please also note that your Test Drive site is for demonstration purposes only. Ostrato reserves the right to fully control the site. Use of this site is restricted to and for Ostrato business purposes ONLY. The site will be automatically deleted after couple of hours.</p>";
            html += "<BR/><BR/><p style='font-size:11px'>This is a system generated email. But we do attend this mailbox regularly.</p>";

            return html;
        }

        private String GetConnectionDetails(User user, TestDrive drive)
        {
            String siteURL = "https://" + user.SiteName + ".cloudapp.net";

            String connectionDetails = "";
            connectionDetails += "<BR><B>URL</B>: <a target='_blank' href='" + siteURL + "'>" + siteURL + "</a>";
            connectionDetails += "<BR/><B>User Name</B>: admin";
            connectionDetails += "<BR/><B>Password</B>: password";

            connectionDetails += "<BR><BR><B>SSH Details</B>";
            connectionDetails += "<BR>" + GetRDPDetails(drive, user.SiteName).Replace("\n", "<BR/>");

            connectionDetails += "<BR/><BR/>On your first login, please bear for couple of minutes for the web application to get initiated.";
            return connectionDetails;
        }

        private String GetRDPDetails(TestDrive drive, String serviceName)
        {
            try
            {
                return File.ReadAllText(StringHelper.CreateQualifiedFileName(drive.LogFolder, "rdpdetails", serviceName + "-rdp.txt"));
            }
            catch (Exception)
            {
            }
            return "";
        }
    }
}