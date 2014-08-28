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

    /// <summary>
    /// Lists all files in path and compares them to the xml file. Generate error if amount mismatches.
    /// Does not verify hashes
    /// </summary>
    public class DetectXmlMismatch : Task
    {
        [Required]
        public string Path { get; set; }
        [Required]
        public string BasePath { get; set; }
        [Required]
        public string XmlFile { get; set; }


        public override bool Execute()
        {
            CheckParams();

            // Read all files in
            var files = new HashSet<string>(_getFilesFiltered(Path));

            var xdoc = XDocument.Load(XmlFile);
            var xmlFiles = new HashSet<string>(xdoc.Descendants("name").Select(e => e.Value));

            // The base path
            var bp = BasePath;
            if (!bp.EndsWith(@"\"))
            {
                bp += "\\";
            }

            var leftRemain = files.Where(f => !xmlFiles.Contains(f.Replace(bp, ""))).ToList();

            var rightRemain = xmlFiles.Where(f => !files.Contains(System.IO.Path.Combine(bp, f))).ToList();

            if (leftRemain.Any() == false && rightRemain.Any() == false)
            {
                return true;
            }


            foreach (var left in leftRemain)
            {
                Log.LogWarning(string.Format("File does not exist in xml: '{0}'", left));
            }

            foreach (var right in rightRemain)
            {
                Log.LogWarning(string.Format("File does not exist on filesystem: '{0}'", right));
            }

            
            Log.LogError("Detected differences in filesystem to xml comparison");
            return false;
        }

        private IEnumerable<string> _getFilesFiltered(string path)
        {
            var list = new List<string>();

            list.AddRange(Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly));

            foreach (var directory in Directory.GetDirectories(path))
            {

                list.AddRange(_getFilesFiltered(directory));

            }

            return list;
        }


        private void CheckParams()
        {
            if (XmlFile == null)
            {
                throw new NullReferenceException("XmlFile is null");
            }

            if (File.Exists(XmlFile) == false)
            {
                throw new FileNotFoundException("Xml file not found", XmlFile);
            }

            if (Path == null)
            {
                throw new NullReferenceException("Path is null");
            }

            if (Directory.Exists(Path) == false)
            {
                throw new FileNotFoundException("Path file not found", Path);
            }

            if (BasePath == null)
            {
                throw new NullReferenceException("BasePath is null");
            }

            if (Directory.Exists(BasePath) == false)
            {
                throw new FileNotFoundException("BasePath file not found", BasePath);
            }

        }


    }


}
