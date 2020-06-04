using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SharedServices.Web.Extensions.HtmlHelper.SideMenu
{
    public static class Item
    {
        public static IHtmlString SideMenuLabel<TModel>(
               this HtmlHelper<TModel> htmlHelper, string label, string icon = null)
        {
            string template = $"<div class='row sidebar-body-item'>"+
                "<div class='col-12 py-2'>" +
                $"<b>{(string.IsNullOrWhiteSpace(icon)? label : $"<span><i class='fa {icon}'></i></span><span class='pl-2'>{label}</span>")}</b>"+
                "</div>" +
                "</div>";
            return template.ToHtmlString();
        }
        public static IHtmlString SideMenuLink<TModel>(
               this HtmlHelper<TModel> htmlHelper, string label, string url = "#", string icon = null)
        {
            string template = $"<div class='row sidebar-body-item'>" +
                "<div class='col-12 py-2'>" +
                $"<a href='{url}'>{(string.IsNullOrWhiteSpace(icon) ? label : $"<span><i class='fa {icon}'></i></span><span class='pl-2'>{label}</span>")}</a>" +
                "</div>" +
                "</div>";
            return template.ToHtmlString();
        }
        private static IHtmlString SideMenuSubMenuItem(SubMenuItem item)
        {
            string display = string.IsNullOrWhiteSpace(item.Icon) ? item.Label : $"<span><i class='fa {item.Icon}'></i></span><span class='pl-2'>{item.Label}</span>";
            string template = $"<li>" +(item.IsLink ? $"<a href='{item.Url}'>{display}</a>" : $"<b>{display}</b>") + "</li>";
            return template.ToHtmlString();
        }
        public static IHtmlString SideMenuSubMenu<TModel>(
               this HtmlHelper<TModel> htmlHelper, string label, string id, IEnumerable<SubMenuItem> submenu, string icon = null)
        {
            string template = $"<div class='row sidebar-body-item'>" +
                "<div class='col-12 py-2'>" +
                $"<a href='#{id}SubMenu' data-toggle='collapse' aria-expanded='false' class='dropdown-toggle'>{(string.IsNullOrWhiteSpace(icon) ? label : $"<span><i class='fa {icon}'></i></span><span class='pl-2'>{label}</span>")}</a>" +
                "</div>" +
                "</div>"+
                $"<div class='collapse row sidebar-body-item submenu' id='{id}SubMenu' data-parent=\"[data-sidebar='body']\">"+
                "<div class='col-12'>"+
                    "<ul class='list-unstyled'>"+
                        (submenu.Select(i=> SideMenuSubMenuItem(i)).Concat()).ToString() +
                    "</ul>"+
                "</div>"+
                "</div>";
            return template.ToHtmlString();
        }
    }
}
