using SharedServices.Configuration.Models;
using SharedServices.FederatedAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServices.Configuration
{
    public interface IConfigurationService
    {
        bool UsingFederatedAuth { get; }
        FederatedIPAuthentication FederatedIPAuthentication { get; }
        WebPageConfiguration WebPageConfiguration { get; }

    }
}
