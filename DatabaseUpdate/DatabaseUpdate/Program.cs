using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DatabaseUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            if (args == null || args.Length != 1)
            {
                Console.ReadKey();
                Environment.Exit(0);
            }

            string url = args[0];
            */
            List<string> urls_ns = new List<string>
            {
                "natural-sciences",
                "astronomy",
                "biological-sciences",
                "chemistry-biochemistry",
                "computer-science",
                "human-ecology",
                "marine-science",
                "mathematics",
                "molecular-biosciences",
                "physics",
                "statistics-scientific-computation",
                "neuroscience",
                "uteach-natural-sciences"
            };

            string ns_url = "http://catalog.utexas.edu/undergraduate/natural-sciences/courses/";
            List<Course> courses = new List<Course>();

            foreach (string s in urls_ns)
            {
                string url = ns_url + s + "/";
                string html = HtmlFetcher(url);
                string xhtml = HtmlToXml(html);
                courses.AddRange(XmlToCourses(xhtml));
            }

            StreamWriter write = new StreamWriter("courses.txt");
            foreach (Course c in courses)
            {
                write.WriteLine(c);
            }
            write.Close();
            
        }

        /// <summary>
        /// Returns html string from specified url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HtmlFetcher(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            string data = null;

            if (res.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = res.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(res.CharacterSet));

                data = reader.ReadToEnd();

                reader.Close();
                stream.Close();
            }
            res.Close();

            return data;
        }

        /// <summary>
        /// Returns xhtml string from html document
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlToXml(string html)
        {
            StringBuilder stringbuild = new StringBuilder();
            StringWriter stringwriter = new StringWriter(stringbuild);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            doc.OptionOutputAsXml = true;
            doc.OptionCheckSyntax = true;
            doc.OptionFixNestedTags = true;

            doc.Save(stringwriter);

            return stringbuild.ToString();
        }

        /// <summary>
        /// Returns a List of Courses from xml string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static List<Course> XmlToCourses(string xml)
        {
            List<Course> courses = new List<Course>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList courseNodes = doc.GetElementsByTagName("h5");

            foreach (XmlNode node in courseNodes)
            {
                string abbr = "";
                string[] number = null;
                string description = "";
                string tccn = null;

                string[] courselines = node.InnerText.Split(new string[] { "&#160;" }, StringSplitOptions.None);

                // Obtaining abbr
                // Checking to see whether abbr has a space (E E)
                if (courselines.Length > 2)
                {
                    abbr = courselines[0] + courselines[1];
                }
                else
                {
                    abbr = courselines[0];
                }

                // Obtaining number
                // Splitting by space, then remove last char (the dot).
                int index = (courselines.Length > 2) ? 2 : 1;
                string courseline_split_dot = courselines[index].Split('.')[0].Split(new string[] { "( " }, StringSplitOptions.None)[0];

                if (courseline_split_dot.Split(' ').Length > 1)
                {
                    number = courseline_split_dot.Split(new string[] {", "}, StringSplitOptions.None);
                }
                else
                {
                    number = new string[] {courseline_split_dot};
                }

                // Obtaining description
                // NextSibling #BLESS
                description = node.NextSibling.InnerText;

                // Obtaining tccn
                // Splitting via regex
                Regex reg = new Regex(@"\(TCCN: |\)");
                string courseline = string.Join(" ", courselines);

                if (courseline.Contains("TCCN"))
                {
                    string[] courseline_split_string = reg.Split(courseline);
                    if (courseline_split_string.Length > 1)
                        tccn = courseline_split_string[1];
                }

                // Object Assemble
                foreach (string n in number)
                {
                    courses.Add(new Course(abbr, n, description, tccn));
                }
            }

            return courses;
        }
    }
}
