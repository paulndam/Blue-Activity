using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam.Models {
    public class OtherUser {
        [Key]

        public int OtheUserId { get; set; }

        public int UserId { get; set; }

        public int CenterId { get; set; }

        public User User { get; set; }

        public Center Center { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}