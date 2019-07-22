using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Core.ViewModels
{
    public class ForgotPasssword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
