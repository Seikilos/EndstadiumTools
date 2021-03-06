﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;


namespace SyncTasks
{
    public class FilterFcivXml : Task
    {
        [Required]
        public string IgnoreFiles { get; set; }
        [Required]
        public string IgnoreDirs { get; set; }
        [Required]
        public string XmlFile { get; set; }

        [Output]
        public int FilesInXml { get; set; }

        private FilterUtils _filterUtils;

        public override bool Execute()
        {
            CheckParams();

            Log.LogMessage(string.Format("Removing files '{0}' and dirs '{1}' from file '{2}", IgnoreFiles, IgnoreDirs, XmlFile));

            // To count the amount of remaining files simply count the occurrence of <File_Entry
            var file = File.ReadAllText(XmlFile);
            var totalFiles = new Regex(Regex.Escape("<FILE_ENTRY")).Matches(file).Count;
            var xml = XDocument.Load(XmlFile);

            
            var toRemoveNodes = xml.Descendants("FILE_ENTRY").Where(e => DeleteThisNode(e.Descendants("name").FirstOrDefault().Value));
            var removeNodes = toRemoveNodes as IList<XElement> ?? toRemoveNodes.ToList();
            var removed = removeNodes.Count;
            removeNodes.Remove();
         

            xml.Save(XmlFile);

            FilesInXml = totalFiles - removed;
            return true;
        }

        private bool DeleteThisNode(string nodeValue)
        {
            if (nodeValue == null)
            {
                return false;
            }

            return _filterUtils.MatchesFilter(nodeValue);
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

            _filterUtils = new FilterUtils(IgnoreFiles, IgnoreDirs);

           
        }
    }
}
