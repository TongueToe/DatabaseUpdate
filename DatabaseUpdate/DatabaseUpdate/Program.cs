using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseUpdate
{
    class Program
    {
        static void Main(string[] args)
        {

            string url = "http://catalog.utexas.edu/undergraduate/";

            string[] urls =
            {
                "undergraduate-studies/courses",

                "architecture/courses",

                "business/courses/business-administration",
                "business/courses/accounting",
                "business/courses/finance",
                "business/courses/business-government-society",
                "business/courses/information-risk-operations-management",
                "business/courses/management",
                "business/courses/marketing",

                "communication/courses/communication",
                "communication/courses/advertising",
                "communication/courses/communication-sciences-disorders",
                "communication/courses/communication-studies",
                "communication/courses/journalism",
                "communication/courses/radio-television-film",

                "education/courses/applied-learning-development",
                "education/courses/curriculum-instruction",
                "education/courses/educational-psychology",
                "education/courses/kinesiology-health-education",
                "education/courses/science",
                "education/courses/special-education",

                "engineering/courses/engineering-studies",
                "engineering/courses/general",
                "engineering/courses/aerospace-mechanics",
                "engineering/courses/biomedical",
                "engineering/courses/chemical",
                "engineering/courses/civil-architectural-environmental",
                "engineering/courses/electrical-computer",
                "engineering/courses/mechanical",
                "engineering/courses/petroleum-geosystems",

                "fine-arts/courses/arts-and-entertainment-technologies",
                "fine-arts/courses/art-and-art-history",
                "fine-arts/courses/theatre-dance",
                "fine-arts/courses/fine-arts",
                "fine-arts/courses/music",

                "geosciences/courses/",

                "information/courses/",

                "liberal-arts/courses/liberal-arts",
                "liberal-arts/courses/liberal-arts-honors",
                "liberal-arts/courses/african-african-diaspora-studies",
                "liberal-arts/courses/american-studies",
                "liberal-arts/courses/anthropology",
                "liberal-arts/courses/asian-studies",
                "liberal-arts/courses/classics",
                "liberal-arts/courses/classical-languages",
                "liberal-arts/courses/classical-studies",
                "liberal-arts/courses/cognitive-science",
                "liberal-arts/courses/comparative-literature",
                "liberal-arts/courses/core-texts-ideas",
                "liberal-arts/courses/amrico-paredes-cultural-studies",
                "liberal-arts/courses/economics",
                "liberal-arts/courses/english",
                "liberal-arts/courses/asian-american-studies",
                "liberal-arts/courses/european-studies",
                "liberal-arts/courses/french-italian",
                "liberal-arts/courses/geography-environment",
                "liberal-arts/courses/germanic-studies",
                "liberal-arts/courses/government",
                "liberal-arts/courses/department-of-health-and-society",
                "liberal-arts/courses/history",
                "liberal-arts/courses/human-dimensions-organizations",
                "liberal-arts/courses/humanities",
                "liberal-arts/courses/international-relations-global-studies",
                "liberal-arts/courses/jewish-studies",
                "liberal-arts/courses/latin-american-studies",
                "liberal-arts/courses/linguistics",
                "liberal-arts/courses/mexican-american-studies",
                "liberal-arts/courses/middle-eastern-studies",
                "liberal-arts/courses/philosophy",
                "liberal-arts/courses/plan-ii-honors-program",
                "liberal-arts/courses/psychology",
                "liberal-arts/courses/religious-studies",
                "liberal-arts/courses/rhetoric-writing",
                "liberal-arts/courses/air-force-science",
                "liberal-arts/courses/military-science",
                "liberal-arts/courses/naval-science",
                "liberal-arts/courses/russian-east-european-eurasian-studies",
                "liberal-arts/courses/slavic-eurasian-studies",
                "liberal-arts/courses/sociology",
                "liberal-arts/courses/spanish-portuguese",
                "liberal-arts/courses/uteach-liberal-arts",
                "liberal-arts/courses/womens-gender-studies",

                "natural-sciences/courses/natural-sciences",
                "natural-sciences/courses/astronomy",
                "natural-sciences/courses/biological-sciences",
                "natural-sciences/courses/chemistry-biochemistry",
                "natural-sciences/courses/computer-science",
                "natural-sciences/courses/human-ecology",
                "natural-sciences/courses/marine-science",
                "natural-sciences/courses/mathematics",
                "natural-sciences/courses/molecular-biosciences",
                "natural-sciences/courses/physics",
                "natural-sciences/courses/statistics-scientific-computation",
                "natural-sciences/courses/neuroscience",
                "natural-sciences/courses/uteach-natural-sciences",

                "nursing/courses",

                "pharmacy/courses",

                "public-affairs/general-information-and-courses",

                "social-work/courses"
            };

            WriteToJson(url, urls);

            /*
            List<Course> courses = WriteToJson(url, urls);

            IMongoClient client = new MongoClient(MongoClientSettings.FromUrl(new MongoUrl("mongodb://admin:123@ds151008.mlab.com:51008/heroku_k0jd41mf")));
            IMongoDatabase database = client.GetDatabase("heroku_k0jd41mf");

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("utcourses");

            foreach (Course c in courses)
            {
                collection.InsertOne(c.ToBsonDocument());
            }
            */
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
                string name = "";
                string description = "";
                string[] topics = null;
                string tccn = null;

                string[] courselines = node.InnerText.Split(new string[] { "&#160;" }, StringSplitOptions.None);

                // Obtaining abbr
                // Checking to see whether abbr has a space (E E)
                if (courselines.Length > 2)
                {
                    abbr = courselines[0] + " " + courselines[1];
                    //abbr = courselines[0] + " " + courselines[1]; <- includes space abbreviations
                }
                else
                {
                    abbr = courselines[0];
                }

                // Obtaining number
                // Splitting by space, then remove last char (the dot).
                int index = (courselines.Length > 2) ? 2 : 1;
                string courseline_split_dot = courselines[index].Split('.')[0].Split(new string[] { " (" }, StringSplitOptions.None)[0];

                if (courseline_split_dot.Split(' ').Length > 1)
                {
                    number = courseline_split_dot.Split(new string[] {", "}, StringSplitOptions.None);
                }
                else
                {
                    number = new string[] {courseline_split_dot};
                }

                // Obtaining name
                courseline_split_dot = courselines[index].Split('.')[1];

                name = courseline_split_dot.Substring(1);

                // Obtaining description
                // NextSibling w u w
                XmlNode nextNode = node.NextSibling;
                description = nextNode.InnerText.Replace("\n", "").Replace("\r", "");

                // Obtaining topics

                if (nextNode.NextSibling != null && nextNode.NextSibling.Name.Equals("p"))
                {
                    string s = "." + nextNode.NextSibling.InnerText.Replace("\n", "").Replace("\r", "");

                    topics = s.Split(new string[] { ".Topic " }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < topics.Length; i++)
                    {
                        topics[i] = "Topic " + topics[i];
                    }

                }

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
                    courses.Add(new Course(abbr, n, name, description, topics, tccn));
                }
            }

            return courses;
        }

        /// <summary>
        /// Write courses from urls to specified outfile
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="url_template"></param>
        public static List<Course> WriteCourses(string[] urls, string url_template, string outfile = "courses.txt")
        {
            List<Course> courses = new List<Course>();

            foreach (string s in urls)
            {
                string ns_url = url_template + s + "/";
                string html = HtmlFetcher(ns_url);
                string xhtml = HtmlToXml(html);
                courses.AddRange(XmlToCourses(xhtml));
            }

            StreamWriter write = new StreamWriter("courses.txt");
            foreach (Course c in courses)
            {
                write.WriteLine(c);
            }
            write.Close();

            return courses;
        }

        public static List<Course> FetchCourses(string name)
        {
            List<Course> courses = new List<Course>();

            StreamReader read = new StreamReader(name);
            string[] lines = read.ReadToEnd().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string l in lines)
            {
                string[] s = l.Replace("\r", "").Replace("\n", "").Split(new String[] { ";;;" }, StringSplitOptions.None);

                if (s[4].Equals(""))
                    courses.Add(new Course(s[0], s[1], s[2], s[3], null, s[5]));
                else
                    courses.Add(new Course(s[0], s[1], s[2], s[3], s[4].Split(new String[] { ";;" }, StringSplitOptions.None), s[5]));
            }

            return courses;
        }

        public static List<Course> WriteToJson(string url, string[] urls)
        {
            List<Course> courses = WriteCourses(urls, url);
            // List<Course> courses = FetchCourses("courses.txt");

            string json = JsonConvert.SerializeObject(courses, Newtonsoft.Json.Formatting.Indented);

            StreamWriter write = new StreamWriter("C:/Users/Tung/JavaScript/courses.json");
            write.WriteLine(json);
            write.Close();

            return courses;
        }
    }
}
