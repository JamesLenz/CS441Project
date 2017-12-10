using System;
namespace cs441_project
{
    public class ForumPostItem
    {
        public ForumPostItem()
        {
            CreatedDateTime  = new DateTime();
            LastEditDateTime = new DateTime();
            isFirstPost      = false;
        }

        public bool isFirstPost { get; set; }
        public string Id { get; set; }
        public string CreatorName { get; set; }
        public string CreatorEmail { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastEditDateTime { get; set; }
        public string Content { get; set; }

        public bool curUserIsCreator
        {
            get
            {
                return App.userEmail.ToLower() == CreatorEmail.ToLower();
            }
        }

        public bool isEdited
        {
            get
            {
                return CreatedDateTime.CompareTo(LastEditDateTime) != 0;
            }
        }

        public bool notEdited
        {
            get
            {
                return CreatedDateTime.CompareTo(LastEditDateTime) == 0;
            }
        }
    }
}
