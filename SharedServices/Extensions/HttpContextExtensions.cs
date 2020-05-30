using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SharedServices.Extensions
{
    internal static class HttpContextExtensions
    {
        public static HttpCookie GetCookie(this HttpContext context, string name) => context.Request.Cookies.Get(name);
        public static bool HasCookie(this HttpContext context, string name) => context.GetCookie(name) != null;

        public static void SetCookie(this HttpContext context, HttpCookie cookie) => context.Response.Cookies.Set(cookie);
        public static void AddCookie(this HttpContext context, HttpCookie cookie) => context.Response.Cookies.Add(cookie);

        /*Browsers will soon stop delivering cookies where SameSite = SameSiteMode.None && Secure = false */
        public static HttpCookie CreateSessionCookie(this HttpContext context, string name, string data) => new HttpCookie(name, data) {
            //Path = "/",
            //Domain = "localhost",
            //SameSite = SameSiteMode.None,
            HttpOnly = true//, 
            //Secure = true
        };
        public static HttpCookie CreateCookie(this HttpContext context, string name, string data, DateTime expires) => new HttpCookie(name, data) {
            //Path = "/",
            //Domain = "localhost",
            //SameSite = SameSiteMode.None,
            HttpOnly = true, 
            //Secure = true,
            Expires = expires};
        public static HttpCookie ExpireCookie(this HttpCookie cookie) {
            cookie.Expires = DateTime.UtcNow.AddDays(-1);
            return cookie;
        }
        public static void RemoveCookie(this HttpContext context, string name) => context.AddCookie(context.GetCookie(name).ExpireCookie());
        public static void RemoveFullCookie(this HttpContext context, string name) => context.Response.Cookies.Remove(name);

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}
