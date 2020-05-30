using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Helpers
{
    public interface ISiteMapItem
    {
        string Key { get; }
        string Name { get; }
        string Action { get; }
        string Url { get; }
        string Controller { get; }
        string Area { get; }
        bool IsPost { get; }
        bool RequiresAuthentication { get; }
        bool HasArea { get; }
        bool IsPage { get; }
        bool IsIgnored { get; }
    }
}
