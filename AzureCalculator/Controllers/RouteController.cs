using AzureCalculator.DAO;
using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureCalculator.Controllers
{
    public class RouteController : Controller
    {
        // GET: Route
        public ActionResult Index(String isvName, String testDriveName)
        {
            if (String.IsNullOrEmpty(isvName) && String.IsNullOrEmpty(testDriveName))
            {
                return View("~/Views/Home/Home.cshtml", new {});
            }

            ISV isv = String.IsNullOrEmpty(isvName) ? null : ISVDAO.GetByName(isvName);
            if (isv == null)
            {
                isv = ISVDAO.GetByName("motifworks");
            }

            ViewBag.ISVId = isv.ISVId;

            if (String.IsNullOrEmpty(testDriveName))
            {
                return View("~/Views/Home/Client.cshtml", new { ISVId = isv.ISVId });
            }

            TestDrive testDrive = TestDriveDAO.GetByName(testDriveName);
            if (testDrive == null)
            {
                testDrive = TestDriveDAO.GetByName("sharepointfarm");
            }

            ViewBag.TestDriveId = testDrive.TestDriveId;

            return View("~/Views/Home/Index.cshtml", new { TestDriveId = testDrive.TestDriveId });
        }
    }
}