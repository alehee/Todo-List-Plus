using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo_List_Plus.Models
{
    class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Models.List> Lists { get; set; } = new();
    }
}
