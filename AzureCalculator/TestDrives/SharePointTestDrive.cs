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
    public class SharePointTestDrive : ITestDrive
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
            return String.IsNullOrEmpty(drive.MailSubject) ? "Motifworks - SharePoint 2013 on Azure Test Drive" : drive.MailSubject;
        }

        public String GetAccessibilityDetails(User user, TestDrive drive)
        {
            String accessDetails = "";
            accessDetails += "<BR>Your SharePoint Farm is ready. Please click <a href='http://" + user.SiteName + ".cloudapp.net'>here</a> to login. Please find the site details.";
            accessDetails += GetConnectionDetails(drive, user);
            accessDetails += "<BR><B>Please Note site will be available for 2 hours from now</B>";
            return accessDetails;
        }

        public String GetMailContent(User user, TestDrive drive)
        {
            String html = "Dear " + user.UserName + ",";

            html += "<BR/><BR/>Thank you for trying Microsoft SharePoint 2013 on Azure Test Drive from Motifworks.";
            html += "<BR/><BR/>Your SharePoint 2013 site is ready. Please find the details below to login: ";
            html += GetConnectionDetails(drive, user);
            html += "<BR/><BR/><p style='font-size:11px'>Please also note that your Test Drive site is for demonstration purposes only. Motifworks reserves the right to fully control the site. Use of this site is restricted to and for Motifworks business purposes ONLY. The site will be automatically deleted after couple of hours.</p>";
            html += "<BR/><BR/>Motifworks is a cloud solutions company that helps migrate your applications and infrastructure to the cloud. If you would like more information or have any questions, please contact Motifworks Inc. at <a href='mailto:info@motifworks.com'>info@motifworks.com</a> or call us at 1-844-MOTIFWORKS.";
            html += "<BR/><BR/><p style='font-size:11px'>This is a system generated email. But we do attend this mailbox regularly. Please feel free to reply to this mail if you want to get in touch with us.</p>";

            return html;
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

        private String GetConnectionDetails(TestDrive drive, User user)
        {
            String siteURL = "http://" + user.SiteName + ".cloudapp.net";
            String adminURL = "http://" + user.SiteName + ".cloudapp.net:20000";

            String connectionDetails = "";
            connectionDetails += "<BR><B>URL</B>: <a target='_blank' href='" + siteURL + "'>" + siteURL + "</a>";
            connectionDetails += "<BR/><B>User Name</B>: corp\\spadmin";
            connectionDetails += "<BR/><B>Password</B>: " + user.LoginPassword;

            connectionDetails += "<BR/><BR/><B>Central Administration URL</B>: <a target='_blank' href='" + adminURL + "'>" + adminURL + "</a>";
            connectionDetails += "<BR/><B>User Name</B>: corp\\spadmin";
            connectionDetails += "<BR/><B>Password</B>: " + user.LoginPassword;

            connectionDetails += "<BR><BR><B>Remote Desktop Details</B>";
            connectionDetails += "<BR>" + GetRDPDetails(drive, user.SiteName).Replace("\n", "<BR/>");

            connectionDetails += "<BR/><BR/>On your first login, please bear for couple of minutes for the web application to get initiated.";
            return connectionDetails;
        }
    }
}