using System;

namespace cs441_project
{
    public class ForumThreadItem
    {
        public ForumThreadItem()
        {
            CreatedDateTime = new DateTime();
            LastPostDateTime = new DateTime();
            NumberOfPosts = "?";
        }

        public ForumThreadItem(ForumThreadItem item)
        {
            Id               = item.Id;
            CreatedDateTime  = item.CreatedDateTime;
            Title            = item.Title;
            Description      = item.Description;
            CreatorName      = item.CreatorName;
            CreatorEmail     = item.CreatorEmail;
            NumberOfPosts    = item.NumberOfPosts;
            LastPostDateTime = item.LastPostDateTime;
        }

        public string Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatorName { get; set; }
        public string CreatorEmail { get; set; }
        public string NumberOfPosts { get; set; }
        public DateTime LastPostDateTime { get; set; }

        public bool curUserIsCreator
        {
            get
            {
                return App.userEmail.ToLower() == CreatorEmail.ToLower();
            }
        }
    }
}

