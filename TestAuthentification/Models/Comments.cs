using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Comments
    {
        public int CommentId { get; set; }
        public int? CommentUserId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public int? CommentVehId { get; set; }
        public int? CommentLocId { get; set; }

        public Location CommentLoc { get; set; }
        public User CommentUser { get; set; }
        public Vehicle CommentVeh { get; set; }
    }
}
