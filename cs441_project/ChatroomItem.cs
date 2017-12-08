using System;
using System.Collections.Generic;

namespace cs441_project
{
    public class ChatroomItem
    {
        public ChatroomItem()
        {
            Members = new List<UserItem>();
        }

        public List<UserItem> Members { get; set; }
        public string Id { get; set; }

        public string ListOfMemberNames
        {
            get
            {
                string temp = "";
                for (int i = 0; i < Members.Count; i++)
                    temp += Members[i].Name + ", ";
                temp = temp.Substring(0, temp.Length - 2); //remove last ", "
                return temp;
            }
        }
    }
}
