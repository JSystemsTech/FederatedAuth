using SharedServices.Web.Attributes;
using SharedServices.Web.Controllers;
using System.IO;
using System.Web.Mvc;
using SharedServices.Web.Extensions;
using FederatedIPWeb.Models;

namespace FederatedIPWeb.Controllers
{

    [SiteMapIgnore]
    public class BaseController : ApplicationBaseController
    {




        //    protected override ActionResult UnauthorizedResult(string message)
        //    {
        //        return View("_Unauthorized", new UnauthorizedVM() { Message = message});
        //    }

        //    protected override PartialViewResult UnauthorizedPartialResult(string message)
        //    {
        //        return PartialView("_UnauthorizedPartial", new UnauthorizedVM() { Message = message });
        //    }
    }
}