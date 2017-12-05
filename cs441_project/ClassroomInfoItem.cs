using System;
namespace cs441_project
{
    public class ClassroomInfoItem
    {
        public ClassroomInfoItem()
        {
            
        }

        public ClassroomInfoItem(ClassroomInfoItem classroom)
        {
            Id          = classroom.Id;
            Title       = classroom.Title;
            Description = classroom.Description;
            OwnerEmail  = classroom.OwnerEmail;
            OwnerName   = classroom.OwnerName;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerName { get; set; }

        //current user is owner of this classroom
        public bool curUserIsOwner
        {
            get
            {
                return OwnerEmail.ToLower() == App.userEmail.ToLower();
            }
        }
    }
}
