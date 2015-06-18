using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using AzureCalculator.Helper;
using AzureCalculator.DAO;
using AzureCalculator.TestDrives;
using System.Threading;
using NLog;

namespace AzureCalculator.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            String testDriveId = HttpContext.Request.Params["TestDriveId"];
            if (String.IsNullOrEmpty(testDriveId))
            {
                testDriveId = "4";
            }
            ViewBag.TestDriveId = testDriveId;
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }
            

        [HttpPost]
        public JsonResult AvailableISV()
        {
            return Json(ISVDAO.GetAll());
        }

        [HttpPost]
        public JsonResult AvailableDrives(int id)
        {
            return Json(ISVDAO.GetAvailableTestDrives(id));
        }

        [HttpPost]
        public JsonResult TestDrive(int id)
        {
            return Json(TestDriveDAO.Get(id));
        }

        [HttpPost]
        public JsonResult ISV(int id)
        {
            return Json(ISVDAO.Get(id));
        }

        [HttpPost]
        public ActionResult Index(FormCollection frm)
        {

            int userId = 0;
            String error = "";
            try
            {
                User user = new User
                {
                    UserName = frm["UserName"],
                    CompanyName = frm["CompanyName"],
                    ContactNumber = frm["ContactNumber"],
                    Email = frm["Email"],
                    LoginPassword = StringHelper.GenerateRandomString(3, 97, 26) + "aT6Rv!" + StringHelper.GenerateRandomString(3, 65, 26) + StringHelper.GenerateRandomString(2, 48, 10),
                    LoginId = frm["LoginId"],
                    TestDriveId = Convert.ToInt32(frm["TestDriveId"])
                };

                userId = UserDAO.Create(user);

                UserDAO.UpdateSiteName(userId, "mw-atd-" + ("" + userId).PadLeft(3, '0'));
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            if (userId > 0)
            {
                TempData["ID"] = userId;
                TempData["ERROR"] = error;
                TempData["TestDriveId"] = frm["TestDriveId"];
                TempData.Keep();
                return RedirectToAction("Setup");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Provision(int id)
        {
            logger.Debug("Provisioning request - " + id);
            User user = UserDAO.Get(id);
            if (user == null || user.UserId == 0)
            {
                logger.Error("Invalid Provisioning Request for " + id);
                return Json(new { success = false, response = "Request not found." }, JsonRequestBehavior.AllowGet);
            }

            logger.Debug("Checking Status for request - " + id);

            if (!user.Status.Equals(StatusType.STATUS_REQUESTED))
            {
                logger.Error("Status not OK for request - " + id + ", Expected: " + StatusType.STATUS_REQUESTED + ", Got: " + user.Status);
                return Json(new { success = false, response = "Duplicate Request." }, JsonRequestBehavior.AllowGet);
            }

            logger.Debug("Request found OK - " + id + ", submitting thread");

            Thread thread = new Thread(() => StartProvision(user));
            thread.Start();

            logger.Debug("Request found OK - " + id + ", submitted thread");
            return Json(new { success = true, response="Provision Completed"}, JsonRequestBehavior.AllowGet);
        }

        private void StartProvision(User user)
        {
            Logger logger = LogManager.GetLogger(user.SiteName, GetType());

            logger.Debug("Provisioning Started.");

            UserDAO.UpdateStatus(user.UserId, StatusType.STATUS_PROVISIONING);
            logger.Debug("User status updated to Provisioning");

            TestDrive drive = TestDriveDAO.Get(user.TestDriveId);
            logger.Debug("Drive fetched.");

            
            logger.Debug("Creating Default Directories...");
            if (!String.IsNullOrEmpty(drive.LogFolder))
            {
                Directory.CreateDirectory(StringHelper.ReplacePathSeparator(drive.LogFolder));
            }
            Directory.CreateDirectory(StringHelper.CreateQualifiedFileName(drive.LogFolder, "rdpdetails"));
            logger.Debug("Default Directories created.");

            String driveStatus = StatusType.STATUS_COMPLETED;

            logger.Debug("Instantiating controller - " + drive.Controller);
            ITestDrive driveController = null;
            try
            {
                driveController = (ITestDrive)Activator.CreateInstance(Type.GetType(drive.Controller));
                logger.Debug("Controller instantiated - " + drive.Controller);
            }catch(Exception e)
            {
                logger.Error("Unable to instantiate controller - " + drive.Controller, e);
                return;
            }

            logger.Debug("Fetching steps for - " + drive);
            List<TestDriveStep> steps = TestDriveDAO.GetSteps(drive.TestDriveId);

            logger.Debug("Fetched steps for - " + steps.Count());

            int stepNo = 0;
            foreach (TestDriveStep step in steps)
            {
                stepNo++;
                logger.Debug("Start Execution Step[" + stepNo + "] - " + step);
                Status status = new Status { UserId = user.UserId, StepId = step.StepId, SequenceNumber = stepNo };

                logger.Debug("Updating Step status for Step[" + stepNo + "] - " + step);
                status = UserDAO.StartStepExecution(status);
                logger.Debug("Step status updated for Step[" + stepNo + "] - " + step);

                logger.Debug("Invoking controller for Step[" + stepNo + "] - " + step);
                String stepRemark = null;
                try
                {
                    stepRemark = driveController.ExecuteStep(user, drive, step);
                    status.StepStatus = StatusType.STATUS_COMPLETED;
                    logger.Debug("Successfully completed Step[" + stepNo + "] - " + step);
                }
                catch (Exception e)
                {
                    stepRemark = "Error occurred - " + e.Message;
                    status.StepStatus = StatusType.STATUS_ERROR;
                    driveStatus = StatusType.STATUS_ERROR;
                    logger.Error("Error occured for Step[" + stepNo + "] - " + step, e);
                }
                status.Remark = String.IsNullOrEmpty(stepRemark) ? "Completed Successfully." : stepRemark;

                logger.Debug("Updating Step status for Step[" + stepNo + "] - " + step + " to " + status);
                UserDAO.EndStepExecution(status);
                logger.Debug("Step status updated for Step[" + stepNo + "] - " + step + " to " + status);

                if (StatusType.STATUS_ERROR.Equals(driveStatus))
                {
                    logger.Debug("Error occured during Step[" + stepNo + "] - " + step + " breaking.");
                    break;
                }
            }

            user.Status = driveStatus;

            UserDAO.UpdateAvailabilityDate(user.UserId, driveStatus);
            logger.Debug("Availability date updated.");

            if (!StatusType.STATUS_ERROR.Equals(driveStatus))
            {
                logger.Debug("Sending Mail ");
                MailHelper.Send(driveController.GetMailSubject(user, drive),
                            user.Email,
                            driveController.GetMailContent(user, drive));
                logger.Debug("Mail Sent");
            }
        }

        [HttpPost]
        public JsonResult Status(int id)
        {
            User user = UserDAO.Get(id);
            if (user == null || user.UserId == 0)
            {
                return Json(new { status = "E", percentage = 0, currentStep = "", remarks = "No such request found.", note = ""}, JsonRequestBehavior.AllowGet);
            }

            TestDrive drive = TestDriveDAO.Get(user.TestDriveId);

            List<TestDriveStep> steps = TestDriveDAO.GetSteps(drive.TestDriveId);

            List<Status> status = UserDAO.GetDriveStatus(id);

            String driveStatus = StatusType.STATUS_PROVISIONING;
            String currentStep = "Step 1 of " + steps.Count() + " " + steps.First().StepName;
            int percentage = 0;
            String remarks = "";

            if (status.Count() > 0)
            {
                TestDriveStep step = steps[status.Count() - 1];

                driveStatus = status.Count() < steps.Count() || status.Last().StepStatus == StatusType.STATUS_PROVISIONING ? StatusType.STATUS_PROVISIONING : StatusType.STATUS_COMPLETED;
                currentStep = "Step " + status.Count() + " of " + steps.Count() + " " + step.StepName;
                percentage = steps[status.Count() - 1].Percentage;

                //Prorate Percentage on the basis of time exhausted
                if (StatusType.STATUS_PROVISIONING.Equals(status.Last().StepStatus))
                {
                    int lastPercentage = status.Count() > 1 ? steps[status.Count() - 2].Percentage : 0;
                    int percentageDiff = percentage - lastPercentage;
                    int timeRequired = step.ExpectedTime;
                    int timeExhausted = Convert.ToInt32((DateTime.Now - status.Last().StartTime.Value).TotalMilliseconds / 1000);
                    if (timeExhausted < timeRequired)
                    {
                        percentageDiff = (percentageDiff * timeExhausted) / timeRequired;
                        percentage = lastPercentage + percentageDiff;
                    }
                }

                int stepCount = 1;
                foreach(Status s in status)
                {
                    if (stepCount > 1)
                    {
                        remarks += "<BR>";
                    }
                    remarks += s.StartTime + " Step " + stepCount + " of " + steps.Count() + " " + steps[stepCount - 1].StepDescription;
                    if (!StatusType.STATUS_PROVISIONING.Equals(s.StepStatus))
                    {
                        remarks += "<BR>" + s.EndTime + " Step " + stepCount + " of " + steps.Count() + " " + steps[stepCount - 1].StepName + " Completed.";
                    }

                    stepCount++;
                }
            }

            if (!StatusType.STATUS_PROVISIONING.Equals(driveStatus))
            {
                ITestDrive driveController = (ITestDrive)Activator.CreateInstance(Type.GetType(drive.Controller));
                remarks += "<BR>" + driveController.GetAccessibilityDetails(user, drive);
                currentStep = "Installation Complete.";
            }

            if (StatusType.STATUS_EXPIRED.Equals(user.Status))
            {
                remarks += "<BR><B>This drive is Expired. Please provision another.</B>";
                currentStep = "Test Drive Expired.";
            }

            int expectedTime = drive.ExpectedTime;
            if (expectedTime <= 0)
            {
                expectedTime = 0;
                foreach(TestDriveStep step in steps)
                {
                    expectedTime += step.ExpectedTime;
                }
            }

            String note = "";
            if (expectedTime > 0)
            {
                note = "<B>Note</B>: It will take about " + Convert.ToInt32(expectedTime / 60) + " minutes to provision your test drive. You will be notified on the provided email address, once the site is created.";
            }

            return Json(new { status = driveStatus, percentage = percentage, currentStep = currentStep, remarks = remarks, note = note}, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult Setup()
        {
            if (TempData != null && TempData["ID"] != null)
            {
                ViewBag.ID = Convert.ToInt32(TempData["ID"]);
                ViewBag.Error = TempData["ERROR"];
                ViewBag.TestDriveId = TempData["TestDriveId"];
            }
            else
            {
                String testDriveId = HttpContext.Request.Params["TestDriveId"];
                if (String.IsNullOrEmpty(testDriveId))
                {
                    testDriveId = "4";
                }
                ViewBag.TestDriveId = testDriveId;
            }
            return View();
        }

        public ActionResult Client()
        {
            ViewBag.ISVId = HttpContext.Request.Params["ISVId"];
            return View();
        }
    }
}
