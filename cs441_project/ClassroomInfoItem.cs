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
            Id = classroom.Id;
            Title = classroom.Title;
            Description = classroom.Description;
            Owner = classroom.Owner;
        }

        public UInt32 Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
    }
}
