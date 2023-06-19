using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Todo_List_Plus.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
