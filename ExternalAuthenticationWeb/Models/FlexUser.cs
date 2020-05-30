using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExternalAuthenticationWeb.Models
{
    public class AuthUser
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }

        public static AuthUser DevUser1 = new AuthUser()
        {
            UserId = new Guid("19a4087c-0ea6-4d2d-8cc2-a4864b620f55"),
            Email = "joe.cool@mycompany.com"
        };
        public static AuthUser DevUser2 = new AuthUser()
        {
            UserId = new Guid("f3c4c691-2099-46f8-8192-037d2886511d"),
            Email = "jane.doe@mycompany.com"
        };
        public static AuthUser DevUser3 = new AuthUser()
        {
            UserId = new Guid("ce99b9b0-e791-4ad0-bb16-c28f8e3381cd"),
            Email = "john.smith@mycompany.com"
        };
        public static AuthUser DevUser4 = new AuthUser()
        {
            UserId = new Guid("f3c4c691-2099-46f8-8192-037d2886511f"),
            Email = "mr.techlead@mycompany.com"
        };
        public static IEnumerable<AuthUser> Users = new AuthUser[4] { DevUser1, DevUser2, DevUser3, DevUser4 };
    }
}