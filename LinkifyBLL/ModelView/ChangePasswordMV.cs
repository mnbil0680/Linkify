using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class ChangePasswordMV
    {
        [Required(ErrorMessage = "Enter a valid Email")]
        [EmailAddress(ErrorMessage = "Enter a valid Email Address")]
        public string Email { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Enter the Password")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm your Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }
    }
}
