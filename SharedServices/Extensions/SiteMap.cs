using SharedServices.Helpers;
using SharedServices.Web.Attributes;
using SharedServices.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SharedServices.Extensions
{
    public static class SiteMap
    {
        private static object[] GetAttributes<TAttribute>(this Type type) where TAttribute : Attribute
        => type.GetCustomAttributes(typeof(TAttribute), true);
        private static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        => type.GetAttributes<TAttribute>().Any();
        private static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        => type.HasAttribute<TAttribute>() ? type.GetAttributes<TAttribute>().First() as TAttribute : null;

        private static object[] GetAttributes<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        => method.GetCustomAttributes(typeof(TAttribute), true);
        internal static IEnumerable<AuthorizeGroupAttribute> GetAuthorizeGroupAttributes(this MethodInfo method)
        => method.GetCustomAttributes<AuthorizeGroupAttribute>().Concat(method.DeclaringType.GetCustomAttributes<AuthorizeGroupAttribute>(true));
        private static bool HasAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        => method.GetAttributes<TAttribute>().Any();
        private static TAttribute GetAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        => method.HasAttribute<TAttribute>() ?  method.GetAttributes<TAttribute>().First() as TAttribute : null;
        private static bool IsAuthentictedType(this Type type)
        => type.HasAttribute<AuthenticatedAttribute>() && type.GetAttribute<AuthenticatedAttribute>().Authenticated;
        private static bool IsAuthentictedMethod(this MethodInfo method, bool defaultValue)
        => method.HasAttribute<AuthenticatedAttribute>() ? method.GetAttribute<AuthenticatedAttribute>().Authenticated : defaultValue;

        internal static bool IsPostMethod(this MethodInfo method)
        => method.HasAttribute<HttpPostAttribute>();
        internal static bool IsAuthentictedMethod(this MethodInfo method)
        => method.IsAuthentictedMethod(method.ReflectedType.IsAuthentictedType());

        internal static bool IsLogoutEndppointMethod(this MethodInfo method)
        => method.HasAttribute<LogoutEndointAttribute>();

        private static bool MethodIsFullPage(this MethodInfo method)
        => method.HasAttribute<SiteMapPageAttribute>() ? method.GetAttribute<SiteMapPageAttribute>().IsPage : true;
        internal static bool MethodReturnsFullPage(this MethodInfo method)
        => method.ReturnType == typeof(ActionResult) && method.MethodIsFullPage();
        internal static string GetMethodDisplayName(this MethodInfo method)
        => method.HasAttribute<SiteMapNameAttribute>() ? method.GetAttribute<SiteMapNameAttribute>().Name : method.Name;
        internal static bool IsIgnorableMethod(this MethodInfo method) => method.IsIgnorableMethod(method.ReflectedType);
        internal static bool IsIgnorableMethod(this MethodInfo method, Type contextController)
        => contextController.HasAttribute<SiteMapIgnoreAttribute>();
        internal static bool IsOrIsSubclassOf<TClass>(this Type type)
        => type == typeof(TClass) || type.IsSubclassOf(typeof(TClass));
        internal static bool IsOnlySubclassOf<TClass>(this Type type)
        => type != typeof(TClass) && type.IsSubclassOf(typeof(TClass));


        public static ISiteMapItem ToSiteMapItem<TMvcApplication>(this MethodInfo method, UrlHelper url, IIntegratedSharedService application)
        {
            return new SiteMapItem(url, method.ReflectedType, method, application.ProjectName);
        }
        private static IEnumerable<KeyValuePair<Type, MethodInfo>> GetSiteMapActions(this Type controller, Type contextController)
        {
            if (contextController.HasAttribute<SiteMapIgnoreAttribute>())
            {
                return new KeyValuePair<Type, MethodInfo>[0];
            }
            IEnumerable<KeyValuePair<Type, MethodInfo>> CurrentMethods = controller
                .GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(method => method.ReturnType.IsOrIsSubclassOf<ActionResult>()).Select( action=> new KeyValuePair<Type, MethodInfo>(contextController, action));
            if (controller.DeclaringType != null && controller.DeclaringType.IsOnlySubclassOf<ApplicationBaseController>())
            {
                CurrentMethods = CurrentMethods.Concat(controller.DeclaringType.GetSiteMapActions(contextController));
            }
            else if (controller.BaseType != null && controller.BaseType.IsOnlySubclassOf<ApplicationBaseController>())
            {
                CurrentMethods = CurrentMethods.Concat(controller.BaseType.GetSiteMapActions(contextController));
            }
            return CurrentMethods;
        }
        //public static IEnumerable<ISiteMapItem> MapSite() => MapSite(new UrlHelper(HttpContext.Request.RequestContext));
        public static IEnumerable<ISiteMapItem> MapSite(this IIntegratedSharedService application, UrlHelper url)
        {
            return application.ApplicationAssembly.ApplicationControllers()
                .SelectMany(controller => controller.GetSiteMapActions(controller))                
                .Select(method => new SiteMapItem(url,method.Key, method.Value, application.ProjectName));

        }
        public static object SerializeMapSite(this IEnumerable<ISiteMapItem> siteMap, bool includeAuthenticated = false)
        {
            IEnumerable<ISiteMapItem> map = siteMap.Where(si => (si.IsPage ? !si.IsPost : true) && !si.IsIgnored && (includeAuthenticated ? true : si.RequiresAuthentication == false));
            IEnumerable<ISiteMapItem> ItemsWithArea = map.Where(si => si.HasArea);
            IEnumerable<ISiteMapItem> ItemsWithoutArea = map.Where(si => !si.HasArea);
            ItemsWithoutArea.GroupBy(i => i.Controller);
            ItemsWithArea.GroupBy(i => i.Controller);
            return new
            {
                Areas = GetSiteAreaMap(ItemsWithArea.GroupBy(i => i.Area)),
                Controllers = GetSiteControllerMap(ItemsWithoutArea.GroupBy(i => i.Controller))
            };
        }
        private static IEnumerable<object> GetSiteActionMap(IEnumerable<ISiteMapItem> items)
        {
            return items.GroupBy(x => x.Key).Select(g => g.First()).Select(item => new { item.Url, Action = item.Key, item.Name, item.IsPost, item.IsPage });
        }
        private static IDictionary<string, IEnumerable<object>> GetSiteControllerMap(IEnumerable<IGrouping<string, ISiteMapItem>> controllersInfo)
        {
            IDictionary<string, IEnumerable<object>> data = new Dictionary<string, IEnumerable<object>>();
            foreach (IGrouping<string, ISiteMapItem> controllerInfo in controllersInfo)
            {
                data.Add(controllerInfo.Key, GetSiteActionMap(controllerInfo));
            }
            return data;
        }
        private static IDictionary<string, IDictionary<string, IEnumerable<object>>> GetSiteAreaMap(IEnumerable<IGrouping<string, ISiteMapItem>> areasInfo)
        {
            IDictionary<string, IDictionary<string, IEnumerable<object>>> data = new Dictionary<string, IDictionary<string, IEnumerable<object>>>();
            foreach (IGrouping<string, ISiteMapItem> areaInfo in areasInfo)
            {
                data.Add(areaInfo.Key, GetSiteControllerMap(areaInfo.GroupBy(i => i.Controller)));
            }
            return data;
        }
    }
}