using SharedServices.Extensions;
using SharedServices.FederatedAuth;
using SharedServices.Helpers;
using SharedServices.Web.Attributes;
using SharedServices.Web.AuthNAuthZ;
using SharedServices.Web.Extensions;
using SharedServices.Web.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace SharedServices.Web.Controllers
{
    [SiteMapIgnore]
    public abstract class ApplicationBaseController: Controller
    {
        protected IEnumerable<ISiteMapItem> SiteMap { get; private set; }
        protected IIntegratedSharedService Application { get => HttpContext.ApplicationInstance as IIntegratedSharedService; }
        protected Version ApplicationVersion { get => Application.ApplicationAssembly.GetName().Version; }

        private HttpContextBase GetHttpContext(ControllerContext context) => context.HttpContext;

        private bool IsAjaxRequest(ControllerContext context) => context.HttpContext.Request.IsAjaxRequest();
        
        protected virtual JsonResult UnauthorizedJsonResult(string error) => Json(new { error }, JsonRequestBehavior.DenyGet);
        private bool AuthenticationRequired(ControllerContext context) 
            => !IsLogoutRequest(context) && ((Application.ConfigurationService.UsingFederatedAuth && Application.ConfigurationService.FederatedIPAuthentication.AuthenticationRequired)
            || context.RequiresAuthentication(Application));

        private bool IsLogoutRequest(ControllerContext context)
            => context.IsLogoutRequest(Application);

        private IWebAuthService WebAuthService { get; set; }

        protected WsFederation GetWsFederationRequest(ControllerContext context) => Application.ConfigurationService.FederatedIPAuthentication.GetWsFederationRequest(GetHttpContext(context));
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Application.ConfigurationService.UsingFederatedAuth)
            {
                WebAuthService = new WebAuthService();
            }
            SiteMap = Application.MapSite(Url);
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
            if (Application.ConfigurationService.UsingFederatedAuth)
            {
                IWebAuthResponse webAuthResponse = WebAuthService.OnAuthentication(GetHttpContext(filterContext), AuthenticationRequired(filterContext));
                if (!webAuthResponse.AllowRequest)
                {
                    if (IsAjaxRequest(filterContext))
                    {
                        Type actionMethodType = filterContext.ActionType(Application);
                        filterContext.Result = actionMethodType == typeof(JsonResult) ? UnauthorizedJsonResult("User is not authenticated") : UnauthorizedPartialResult("User is not authenticated");
                    }
                    else
                    {                           
                        filterContext.Result = ApplicationLogoutWithRedirect();
                    }
                }
            }            
        }
        
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (Application.ConfigurationService.UsingFederatedAuth)
            {
                

                IEnumerable<AuthorizeGroupAttribute> methodGroupAuthorizations = filterContext.GetAuthorizeGroupAttributes(Application);
                IWebAuthResponse webAuthResponse = WebAuthService.OnAuthorization(GetHttpContext(filterContext), methodGroupAuthorizations);
                if (filterContext.Result is HttpUnauthorizedResult || !webAuthResponse.AllowRequest)
                {
                    OnActionExecutingOrUnauthorizedRedirect(filterContext);
                    Type actionMethodType = filterContext.ActionType(Application);
                    filterContext.Result = actionMethodType == typeof(PartialViewResult) ? UnauthorizedPartialResult("User is not authorized") :
                        actionMethodType == typeof(JsonResult) ? UnauthorizedJsonResult("User is not authorized") : UnauthorizedResult("User is not authorized");
                }
                else
                {
                    ViewBag.__AuthorizedPage = true;
                }
            }
                
        }
        private object GetPageConfig(ControllerContext context)
        {
            bool isAuthenticated = AuthenticationRequired(context);
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            int timeout = Application.ConfigurationService.UsingFederatedAuth ? Application.ConfigurationService.FederatedIPAuthentication.Timeout : Application.ConfigurationService.WebPageConfiguration.Timeout;
            return new {
                ApplicationConfiguration = Application.ConfigurationService,
                AuthenticatedPage = isAuthenticated,
                UseTimeout = isAuthenticated && timeout > 0,
                TimeoutConfig = new {
                    Timeout = timeout * 60000,
                    WarningTimeout = (timeout - Application.ConfigurationService.WebPageConfiguration.WarningModalDuration) * 60000,
                    RefreshThreshold = timeout * 60000 * Application.ConfigurationService.WebPageConfiguration.ServerRefreshThreshold,
                    RefreshUrl = Url.Action($"Refresh{(isAuthenticated ? "Authenticated" : "")}", controllerName)
                },
                LogoutUrl = Url.Action("ApplicationLogout", controllerName),
                SiteMap = SiteMap.SerializeMapSite(isAuthenticated),
                HttpStatusCode = new { 
                    Unauthorized = (int)HttpStatusCode.Unauthorized,
                    Forbidden = (int)HttpStatusCode.Forbidden
                }
            };
            //return JsonSerializer.Serialize(config);
        }
        private void OnActionExecutingOrUnauthorizedRedirect(ControllerContext context)
        {
            object config = GetPageConfig(context);
            ViewBag.ApplicationVersion = ApplicationVersion;

            ViewBag.__IsAuthenticatedPage = AuthenticationRequired(context);
            if (ViewBag.__IsAuthenticatedPage)
            {
                ViewBag.__User = User;
            }
            ViewBag.__ApplicationConfiguration = Application.ConfigurationService;
            ViewBag.__ApplicationPageConfigurationJson = JsonSerializer.Serialize(config);
        }
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            OnActionExecutingOrUnauthorizedRedirect(context);
        }
        [Authenticated]
        public  JsonResult GetSiteMapJsonAuthenticated() => Json(SiteMap.SerializeMapSite(true), JsonRequestBehavior.AllowGet);
        public  JsonResult GetSiteMapJson() => Json(SiteMap.SerializeMapSite(), JsonRequestBehavior.AllowGet);

        [Authenticated]
        public JsonResult RefreshAuthenticated() => Refresh();
        public JsonResult Refresh()
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult AuthenticateUser(
            Uri consumingAppUri,
            string email,
            string name,
            IEnumerable<string> roles = null,
            IEnumerable<string> groups = null,
            IEnumerable<string> sites = null)
        {
            string viewStr = WebAuthService.AuthenticateUser(GetHttpContext(ControllerContext), consumingAppUri, email, name, roles, groups, sites);

            return Content(viewStr);
        }
        private ActionResult ApplicationLogoutWithRedirect()
        {
            string viewStr = WebAuthService.Logout(GetHttpContext(ControllerContext));
            return Content(viewStr);
        }
        [LogoutEndoint]
        public ActionResult ApplicationLogout()
        {
            string viewStr = WebAuthService.Logout(GetHttpContext(ControllerContext), true);
            return Content(viewStr);
        }
        protected string RenderViewToString(string viewName, object model = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        protected AjaxPartialViewResult AjaxPartialView(string viewName, object model, string successViewName = null, object successModel = null)
        {
            bool hasSuccessView = !string.IsNullOrWhiteSpace(successViewName);

            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid && hasSuccessView)
                {
                    return AjaxPartialViewResult.ToAjaxPartialViewResult(Json(new
                    {
                        error = false,
                        hasHtml = true,
                        html = PartialView(successViewName, successModel).RenderToString(),
                        message = ""
                    }, JsonRequestBehavior.AllowGet));
                }
                return AjaxPartialViewResult.ToAjaxPartialViewResult(Json(new
                {
                    error = !ModelState.IsValid,
                    hasHtml = true,
                    html = PartialView(viewName, model).RenderToString(),
                    message = ""
                }, JsonRequestBehavior.AllowGet));
            }
            else if (ModelState.IsValid && hasSuccessView)
            {
                return AjaxPartialViewResult.ToAjaxPartialViewResult(PartialView(successViewName, successModel));
            }
            return AjaxPartialViewResult.ToAjaxPartialViewResult(PartialView(viewName, model));
        }
        protected ContentResult LoadJsonFile(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(Server.MapPath(path)))
                {
                    return Content(sr.ReadToEnd(), "application/json");
                }
            }
            catch
            {
                return Content(null, "application/json");
            }

        }
        protected ContentResult LoadContentJsonFile(string name) => LoadJsonFile($"~/Content/json/{name}.json");

        private string UnauthorizedTemplate(string message) => $"<h1>Unauthorized</h1><p>You are unauthorized to access this content</p><p>{message}</p>";
        protected virtual ActionResult UnauthorizedResult(string message)
        =>  Content($"<html><body>{UnauthorizedTemplate(message)}</body></html>");
        
        protected virtual ActionResult UnauthorizedPartialResult(string message) => Content(UnauthorizedTemplate(message));
    }
}
