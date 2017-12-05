using System;

namespace cs441_project
{
    public class TodoItem
    {
        public TodoItem()
        {
            CreatedDateTime = new DateTime();
            DueDateTime = new DateTime();
            HasDueDate = true;
        }

        public TodoItem(TodoItem item)
        {
            Id              = item.Id;
            CreatedDateTime = item.CreatedDateTime;
            DueDateTime     = item.DueDateTime;
            Title           = item.Title;
            Description     = item.Description;
            Done            = item.Done;
            HasDueDate      = item.HasDueDate;
        }

        public string Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime DueDateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public bool HasDueDate { get; set; }
    }
}
