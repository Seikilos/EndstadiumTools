using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnitTests.Data
{
    public class CsProjectData
    {
        private List<string> projectReferences = new List<string>();


        public override string ToString()
        {
            return new XElement("Project", new XElement("ItemGroup", projectReferences.Select(p => new XElement("ProjectReference", new XAttribute("Include", p))))).ToString();

        }

        public CsProjectData AddProjectReference(string reference)
        {
            projectReferences.Add(reference);
            return this;
        }
    }
}
