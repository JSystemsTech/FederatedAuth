using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Web.Extensions.HtmlHelper.SideMenu
{
    public class SubMenuItem
    {
        public string Label { get; private set; }
        public string Url { get; private set; }
        public string Icon { get; private set; }
        public bool IsLink { get=> !string.IsNullOrWhiteSpace(Url); }

        public SubMenuItem(string label, string icon = null)
        {
            Label = label;
            Icon = icon;
        }
        public SubMenuItem(string label, string url, string icon = null): this(label, icon)
        {
            Url = url;
        }
    }
}
