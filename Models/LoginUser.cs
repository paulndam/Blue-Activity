using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam.Models {
    public class LoginUser {
        [Required (ErrorMessage = "email required")]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required (ErrorMessage = "password required")]
        [DataType ("Password")]
        [MinLength (4, ErrorMessage = "4 or more characters")]
        public string LoginPassword { get; set; }
    }
}