using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseUpdate
{
    class Course
    {
        public Course(string abbr, string num, string description, string tccn = null)
        {
            Abbr = abbr;
            Number = num;
            Description = description;
        }

        public string Abbr { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Tccn { get; set; }

        public int GetHours()
        {
            int hour = 0;
            if (int.TryParse(Number.Substring(0,1), out hour))
            {
                return hour;
            }
            return -1;
        }

        public override string ToString()
        {
            return string.Join(" ", new string[] { Abbr, Number, Description, Tccn });
        }
    }
}
