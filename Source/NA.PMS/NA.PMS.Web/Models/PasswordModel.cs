using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NA.PMS.Web.Models
{
    public class PasswordModel
    {
        [Required(ErrorMessage="Old password is required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmation password is required")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

        public string UserName { get; set; }       
        public string Email { get; set; }       
        public string MobileNo { get; set; }

        public string PasswordMessage { get; set; }
        public string PasswordSuccessMessage { get; set; }
        public int HiddenVal { get; set; }
    }
}
