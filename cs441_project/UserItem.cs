using System;
namespace cs441_project
{
    public class UserItem
    {
        public UserItem()
        {
            isOwner = false;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool isOwner { get; set; }
    }
}
