using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Models
{
    public class LoginViewModel
    {
        //[Required(ErrorMessage="Username is required")]
        [Display(Name = "Username")]
        [MaxLength(15)]
        public string UserName {get;set;}

        public string Email { get; set; }

        //[Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }
        
        [Display(Name = "OTP")]
        public string OTP { get; set; }
    }
}