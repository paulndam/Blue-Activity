using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam.Models {
    public class Center {

        [Key]

        public int CenterId { get; set; }

        [Required (ErrorMessage = "title required")]
        [MinLength (2)]
        public string Title { get; set; }

        [Required (ErrorMessage = "description required")]
        [MinLength (2)]
        public string Description { get; set; }

        public int Duration { get; set; }

        public DateTime ActivityTime { get; set; }

        [Required (ErrorMessage = "Date required")]
        [DataType (DataType.Date)]
        [FutureDate]
        public DateTime Time { get; set; }

        //implementing the id of the fan in our game class since a fan can have many games or go to many games
        public int UserId { get; set; }

        //this is the link or connect that get us to our fan model or class
        public User MyCenter { get; set; }

        public List<OtherUser> OtherUsersComingToCenter { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}