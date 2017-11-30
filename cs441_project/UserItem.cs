using System;
namespace cs441_project
{
    public class UserItem
    {
        public UserItem()
        {
            isOwner = false;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public bool isOwner { get; set; }
        public bool isNotOwner { get { return !isOwner; } }
    }
}
