using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class LoginViewModel_PIS
    {
        [Display(Name = "Username")]
        [MaxLength(25)]
        public string UserName { get; set; }

        public string Email { get; set; }

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
