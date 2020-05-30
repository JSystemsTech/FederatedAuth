using SharedServices.Configuration.Models;
using SharedServices.Extensions;
using SharedServices.FederatedAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace SharedServices.Configuration
{
    internal class ConfigurationService: IConfigurationService
    {
        private NameValueCollection GetSection(string sectionName)
        => ConfigurationManager.GetSection(sectionName) is NameValueCollection section && section != null ? section : null;
        private bool HasSection(string sectionName)
        => GetSection(sectionName) != null;


        public bool UsingFederatedAuth { get => HasSection("federatedAuthSettings"); }

        public FederatedIPAuthentication FederatedIPAuthentication { get => UsingFederatedAuth ? new FederatedIPAuthentication(GetSection("federatedAuthSettings")) : null; }

        public WebPageConfiguration WebPageConfiguration { get => HasSection("webPageSettings") ? new WebPageConfiguration(GetSection("webPageSettings")) : new WebPageConfiguration(); }
    }
}
