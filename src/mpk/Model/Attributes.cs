using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.mpk.Model
{
    public class Attributes
    {
        public string Theme { get; set; } = "";

        public string Author { get; set; } = "";

        public string Organization { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Comment { get; set; } = "";

        public string CurrentDate => DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
    }
}
