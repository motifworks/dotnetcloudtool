using AzureCalculator.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace AzureCalculator.Controllers
{
    public class APIController : Controller
    {
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
    }
}