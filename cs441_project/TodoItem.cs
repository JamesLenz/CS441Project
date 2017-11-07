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

        public string Name { get; set; }
        public string Notes { get; set; }
        public bool Done { get; set; }
    }
}
