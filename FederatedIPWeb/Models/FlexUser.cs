using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FederatedIPWeb.Models
{
    public class FlexUser
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> Sites { get; set; }

        public static FlexUser DevUser1 = new FlexUser()
        {
            UserId = new Guid("19a4087c-0ea6-4d2d-8cc2-a4864b620f55"),
            Name = "Joe Cool",
            Email = "joe.cool@mycompany.com",
            Roles = new string[2] { "DevAdmin", "Supervisor" },
            Groups = new string[3] { "Admin", "Manager", "User" },
            Sites = new string[0] { }
        };
        public static FlexUser DevUser2 = new FlexUser()
        {
            UserId = new Guid("f3c4c691-2099-46f8-8192-037d2886511d"),
            Name = "Jane Doe",
            Email = "jane.doe@mycompany.com",
            Roles = new string[1] { "Supervisor" },
            Groups = new string[2] { "Manager", "User" },
            Sites = new string[0] {  }
        };
        public static FlexUser DevUser3 = new FlexUser()
        {
            UserId = new Guid("ce99b9b0-e791-4ad0-bb16-c28f8e3381cd"),
            Name = "John Smith",
            Email = "john.smith@mycompany.com",
            Roles = new string[1] { "Employee" },
            Groups = new string[1] { "User" },
            Sites = new string[1] { "MyOtherSite" }
        };
        public static FlexUser DevUser4 = new FlexUser()
        {
            UserId = new Guid("f3c4c691-2099-46f8-8192-037d2886511f"),
            Name = "Mr Techlead",
            Email = "mr.techlead@mycompany.com",
            Roles = new string[1] { "Supervisor" },
            Groups = new string[1] { "User" },
            Sites = new string[0] { }
        };
        public static IEnumerable<FlexUser> FlexUsers = new FlexUser[4] { DevUser1, DevUser2, DevUser3, DevUser4 };
    }
}