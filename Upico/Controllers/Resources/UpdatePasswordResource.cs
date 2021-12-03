using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class UpdatePasswordResource
    {
        public string UserName { get; set; }

      //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{6,20}$", ErrorMessage = "Invalid Password")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{6,20}", ErrorMessage = "Password is not inform")]
        public string CurrentPassword { get; set; }

        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{6,20}", ErrorMessage = "New password is not inform")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match")]
        public string NewPasswordConfirm { get; set; }
    }
}
