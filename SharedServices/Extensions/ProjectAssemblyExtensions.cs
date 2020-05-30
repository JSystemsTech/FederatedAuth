using SharedServices.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace SharedServices.Extensions
{
    public static class ProjectAssemblyExtensions
    {
        internal static IEnumerable<Type> ApplicationControllers(this Assembly ApplicationAssembly) => ApplicationAssembly.GetTypes().Where(type => type != null && type.IsOrIsSubclassOf<Controller>());


        private static string GetRouteValueDictionaryValue(this RouteValueDictionary routeValueDictionary, string key)
        {
            object value = null;
            routeValueDictionary.TryGetValue(key, out value);
            return value != null && (value is string) ? value.ToString() : null;
        }

        internal static bool RequiresAuthentication(this RequestContext requestContext, IIntegratedSharedService application)
        {
            return requestContext.GetActionMethod(application).IsAuthentictedMethod();
        }
        internal static bool RequiresAuthentication(this ControllerContext controllerContext, IIntegratedSharedService application)
        {
            return controllerContext.GetActionMethod(application).IsAuthentictedMethod();
        }
        internal static bool IsLogoutRequest(this ControllerContext controllerContext, IIntegratedSharedService application)
        {
            return controllerContext.GetActionMethod(application).IsLogoutEndppointMethod();
        }
        internal static IEnumerable<AuthorizeGroupAttribute> GetAuthorizeGroupAttributes(this ControllerContext controllerContext, IIntegratedSharedService application)
        => controllerContext.GetActionMethod(application).GetAuthorizeGroupAttributes();
        internal static Type ActionType(this ControllerContext controllerContext, IIntegratedSharedService application)
        {
            return controllerContext.GetActionMethod(application).ReturnType;
        }
        private static MethodInfo GetActionMethod(this RequestContext requestContext, IIntegratedSharedService application)
        {
            Type controllerType = requestContext.GetControllerFromContext(application);

            ControllerContext controllerContext = new ControllerContext(requestContext, Activator.CreateInstance(controllerType) as ControllerBase);
            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, controllerContext.RouteData.Values.GetRouteValueDictionaryValue("action").ToString());
            return (actionDescriptor as ReflectedActionDescriptor).MethodInfo;
        }
        private static MethodInfo GetActionMethod(this ControllerContext controllerContext, IIntegratedSharedService application)
        {
            Type controllerType = controllerContext.RequestContext.GetControllerFromContext(application);
            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, controllerContext.RouteData.Values.GetRouteValueDictionaryValue("action").ToString());
            return (actionDescriptor as ReflectedActionDescriptor).MethodInfo;
        }
        private static Type GetControllerFromContext(this RequestContext requestContext, IIntegratedSharedService application)
        {
            string controller = requestContext.RouteData.Values.GetRouteValueDictionaryValue("controller").ToString();
            string area = requestContext.RouteData.DataTokens.GetRouteValueDictionaryValue("area");

            string controllerName = GetFullName(application.ProjectName, controller, area);

            Type ty = application.ApplicationAssembly.ApplicationControllers().FirstOrDefault(t => t.FullName == controllerName);
            return ty;
        }
        private static string GetFullName(string projectName, string controller, string area = null)
        => !string.IsNullOrEmpty(area) ? $"{projectName}.Areas.{area}.Controllers.{controller}Controller" : $"{projectName}.Controllers.{controller}Controller";


        internal static string GetControllerName(this Type ControllerType)
        => ControllerType.Name.Replace("Controller", "");
        internal static string GetAreaName(this Type ControllerType, string projectName)
        => ControllerType.Namespace.ToString().Replace(projectName + ".", "").Replace("Areas.", "").Replace(".Controllers", "").Replace("Controllers", "");
    }
}