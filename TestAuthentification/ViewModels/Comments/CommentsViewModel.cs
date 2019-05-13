using System;

namespace TestAuthentification.ViewModels.Comments
{
    public class CommentsViewModel
    {
        public string FriendlyName { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime DatePublication { get; set; }
    }
}