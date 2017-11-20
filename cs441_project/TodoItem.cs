using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace cs441_project
{
    public class TodoItem
    {
        //public int ID { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime DueDateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
    }
}
