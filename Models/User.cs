using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Exam.Models.StrongPassword;

namespace Exam.Models {
    public class User {

        [Key]

        public int UserId { get; set; }

        [Required (ErrorMessage = "firstname required")]
        [MinLength (2)]
        public string Firstname { get; set; }

        [Required (ErrorMessage = "lastname required")]
        [MinLength (2)]
        public string Lastname { get; set; }

        [EmailAddress]
        [Required (ErrorMessage = "email required")]

        public string Email { get; set; }

        [StrongPassword]
        [Required (ErrorMessage = "password required")]
        [MinLength (8)]
        [DataType (DataType.Password)]

        public string Password { get; set; }

        [Required (ErrorMessage = "No password match")]
        [Compare ("Password")]
        [DataType (DataType.Password)]
        [MinLength (8)]
        [NotMapped]

        public string ConfirmPassword { get; set; }

        public List<Center> ActivityOfTheUser { get; set; }

        public List<OtherUser> TheOtherUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime Updated { get; set; } = DateTime.Now;

    }
}