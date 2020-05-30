using SharedServices.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    
    public class DashboardController : AuthenticatedController
    {
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeGroup("Admin")]
        public ActionResult Admin()
        {
            return View();
        }

        [AuthorizeGroup("Manager")]
        public ActionResult Manager()
        {
            return View();
        }
        [Authorize(Roles = "Supervisor")]
        public ActionResult Supervisor()
        {
            return View();
        }
    }
}