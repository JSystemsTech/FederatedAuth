using Microsoft.AspNetCore.Mvc.Routing;
using SharedServices.FederatedAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FederatedIPWeb.Models
{
    public class FlexLogin
    {
        public IEnumerable<FlexUser> Users { get; set; }
        public string ReturnUrl { get; private set; }

        public FlexLogin(IEnumerable<FlexUser> users, string returnUrl)
        {
            Users = users;
            ReturnUrl = returnUrl;
        }
    }
}