using System;

namespace cs441_project
{
    public class TodoItem
    {
        public TodoItem()
        {
        }

        public DateTime CreatedDateTime { get; set; }
        public DateTime DueDateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
    }
}
