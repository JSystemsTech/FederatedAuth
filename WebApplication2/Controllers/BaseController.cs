using SharedServices.Web.Attributes;
using SharedServices.Web.Controllers;
using System.IO;
using System.Web.Mvc;
using SharedServices.Web.Extensions;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{

    [SiteMapIgnore]
    public class BaseController : ApplicationBaseController
    {

        protected override ActionResult UnauthorizedResult(string message)
        {
            return View("_Unauthorized", new UnauthorizedVM() { Message = message });
        }

        protected override ActionResult UnauthorizedPartialResult(string message)
        {
            return PartialView("_UnauthorizedPartial", new UnauthorizedVM() { Message = message });
        }
    }
}