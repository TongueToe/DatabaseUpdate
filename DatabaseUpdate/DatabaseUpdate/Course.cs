using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseUpdate
{
    class Course
    {
        public Course(string abbr, string num, string name, string description, string[] topics, string tccn = null)
        {
            Abbr = abbr;
            Number = num;
            Name = name;
            Description = description.Split('{')[0];
            Tccn = tccn;

            Hours = int.Parse(Number.Substring(0, 1));
            Topics = topics;
        }

        public string Abbr { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tccn { get; set; }
        public int Hours { get; set; }
        public string[] Topics { get; set; }

        public override string ToString()
        {
            if (Topics == null)
            {
                return string.Join(";;;", new string[] { Abbr, Number, Name, Description, null, Tccn });
            }
            else
            {
                return string.Join(";;;", new string[] { Abbr, Number, Name, Description, string.Join(";;", Topics), Tccn });
            }
        }
    }
}
