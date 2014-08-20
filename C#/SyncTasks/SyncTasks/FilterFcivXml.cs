using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;


namespace SyncTasks
{
    public class FilterFcivXml : Task
    {
        public string IgnoreFiles { get; set; }
        public string IgnoreDirs { get; set; }
        public string XmlFile { get; set; }

        private List<string> _fileFilter = new List<string>();
        private List<string> _dirFilter = new List<string>();

        public override bool Execute()
        {
            CheckParams();

            Log.LogMessage(string.Format("Removing files '{0}' and dirs '{1}' from file '{2}", IgnoreFiles, IgnoreDirs, XmlFile));

            var xml = XDocument.Load(XmlFile);

            xml.Descendants("FILE_ENTRY").Where(e => DeleteThisNode(e.Descendants("name").FirstOrDefault().Value)).Remove();
         

            xml.Save(XmlFile);

            return true;
        }

        private bool DeleteThisNode(string nodeValue)
        {
            if (nodeValue == null)
            {
                return false;
            }

            var filterFound = _fileFilter.Any(f => nodeValue.Contains(f.Replace("*", "")));

            if (filterFound)
            {
                return true;
            }

            return _dirFilter.Any(d => nodeValue.Contains(d.Replace("*", "")+"\\"));

        }

        private void CheckParams()
        {
            if (XmlFile == null)
            {
                throw new NullReferenceException("XmlFile is null");
            }

            if (IgnoreDirs == null)
            {
                throw new NullReferenceException("IgnoreDirs is null");
            }
            if (IgnoreFiles == null)
            {
                throw new NullReferenceException("IgnoreFiles is null");
            }

            if (File.Exists(XmlFile) == false)
            {
                throw new FileNotFoundException("Xml file not found", XmlFile);
            }

            _fileFilter = IgnoreFiles.Trim().Split(' ').ToList();
            _dirFilter = IgnoreDirs.Trim().Split(' ').ToList();
        }
    }
}
