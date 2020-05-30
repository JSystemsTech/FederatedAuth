using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExternalAuthenticationWeb.Models
{
    public class UserAuth
    {
        public string ReturnUrl { get; set; }
        [Required]
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}