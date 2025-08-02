using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public  class VerifyEmailMV
    {
        [Required(ErrorMessage = "Enter a valid Email")]
        [EmailAddress(ErrorMessage = "Enter a valid Email Address")]
        public string Email { get; set; }
    }
}
