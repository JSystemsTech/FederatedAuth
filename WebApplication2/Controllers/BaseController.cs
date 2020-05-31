using SharedServices.Web.Attributes;
using SharedServices.Web.Controllers;
using System.IO;
using System.Web.Mvc;
using SharedServices.Web.Extensions;
using WebApplication2.Models;
using System.Web.Routing;
using WebApplication2.EmailService;

namespace WebApplication2.Controllers
{

    [SiteMapIgnore]
    public class BaseController : ApplicationBaseController
    {
        protected WebAppEmailService EmailService { get; private set; }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            EmailService = new WebAppEmailService(Application.ConfigurationService.EmailService);
        }
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