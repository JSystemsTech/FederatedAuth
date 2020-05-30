using SharedServices.Web.Attributes;
using System.Web.Mvc;
using SharedServices.Web.Helpers;

namespace WebApplication2.Controllers
{
    [Authenticated]
    [SiteMapIgnore]
    public class AuthenticatedController : BaseController { }
}