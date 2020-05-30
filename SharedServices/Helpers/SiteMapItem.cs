using SharedServices.Extensions;
using System;
using System.Reflection;
using System.Web.Mvc;

namespace SharedServices.Helpers
{

    internal class SiteMapItem : ISiteMapItem
    {
        public string Key { get => $"{Action}{(IsPage ? "Page":"")}{(IsPost ? "Post" : "")}"; }
        public string Name { get; private set; }
        public bool IsPage { get; private set; }
        public string Action { get => Method.Name; }
        public string Controller { get; private set; }
        public string Area { get; private set; }
        public string Url { get; private set; }
        public bool IsPost { get; private set; }
        public bool RequiresAuthentication { get; private set; }
        public bool HasArea { get => !string.IsNullOrWhiteSpace(Area); }
        public bool IsIgnored { get; private set; }

        private string ProjectName { get; set; }
        internal MethodInfo Method { get; set; }
        internal Type ControllerType { get; set; }


        private string GetControllerName()
        => ControllerType.GetControllerName();
        private string GetAreaName()
        => ControllerType.GetAreaName(ProjectName);
        public SiteMapItem(UrlHelper url, Type contextController, MethodInfo method, string projectName)
        {
            ControllerType = contextController;            
            Method = method;            
            ProjectName = projectName;
            Controller = GetControllerName();
            Area = GetAreaName();
            Name = Method.GetMethodDisplayName();
            IsPage =  Method.MethodReturnsFullPage();
            IsPost = Method.IsPostMethod();
            RequiresAuthentication = Method.IsAuthentictedMethod();
            IsIgnored = Method.IsIgnorableMethod(ControllerType);
            Url = HasArea ? url.Action(Action, Controller, new { Area }) : url.Action(Action, Controller);
        }

    }
}