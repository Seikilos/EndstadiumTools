using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using SyncTasks;

namespace SyncTasksTests
{
    [TestFixture]
    public class DetectXmlMismatchTests
    {
        private string _dir = "testdata";

        private string _xmlName = "foo.xml";

        public static void WriteXml(string path, IEnumerable<Tuple<string, string>> files)
        {
            var doc = new XDocument();
            var e = new XElement("FCIV");
            doc.Add(e);

            foreach (var tuple in files)
            {
                var ex = new XElement("FCIV_ENTRY");
                ex.Add(tuple);

                e.Add(ex);
            }

            doc.Save(path);

        }

        [SetUp]
        public void SetUp()
        {
          /*  TearDown();

            Directory.CreateDirectory(_dir);

            File.WriteAllText(Path.Combine(_dir, "a.txt"), "");
            File.WriteAllText(Path.Combine(_dir, "b.txt"), "");
            File.WriteAllText(Path.Combine(_dir, "c.txt"), "");
            File.WriteAllText(Path.Combine(_dir, "d.txt"), "");


            WriteXml(Path.Combine(_dir, _xmlName), new List<Tuple<string, string>>
                {
                    Tuple.Create(Path.Combine(_dir, "a.txt"), "x"),
                    Tuple.Create(Path.Combine(_dir, "b.txt"), "x"),
                    Tuple.Create(Path.Combine(_dir, "e.txt"), "x"),
                });

            */
        }

        [TearDown]
        public void TearDown()
        {
   /*        Directory.Delete(_dir, true); */
        }


        [Test]
        public void Test_Left_And_Right_Distinct_Files()
        {
            var files = new HashSet<string>
                {
                    Path.Combine(_dir, "a.txt"),
                    Path.Combine(_dir, "b.txt"),
                    Path.Combine(_dir, "c.txt")
                };


            var xmlEntries = new HashSet<string>
                {
                     "a.txt",
                     "b.txt",
                     "e.txt",
                };

            var leftRemain = files.Where(f => !xmlEntries.Contains(f.Replace(_dir+@"\",""))).ToList();
            
            var rightRemain = xmlEntries.Where(f => !files.Contains(Path.Combine(_dir, f))).ToList();


        }

    }
}
