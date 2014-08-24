using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyncTasks
{
    /// <summary>
    /// Helper determining whether a string should be filtered
    /// </summary>
    public class FilterUtils
    {
        public string FileFilter { get; private set; }
        public string DirFilter { get; private set; }


        private List<Regex> _fileFilter = new List<Regex>();
        private List<Regex> _dirFilter = new List<Regex>();

        public FilterUtils(string fileFilter, string dirFilter)
        {
            FileFilter = fileFilter;
            DirFilter = dirFilter;

            if (FileFilter != null)
            {
                // Separates entries
                var ff = Regex.Matches(FileFilter.Trim(), @"[\""].+?[\""]|[^ ]+").Cast<Match>().Select(m => m.Value).ToList();

                // Create Regex
                foreach (var str in ff)
                {
                    _fileFilter.Add(new Regex(string.Format("\\\\{0}$", _sanitizeInput(str)), RegexOptions.Compiled | RegexOptions.IgnoreCase));
                }
            }


            if (DirFilter != null)
            {
                var df = Regex.Matches(DirFilter.Trim(), @"[\""].+?[\""]|[^ ]+").Cast<Match>().Select(m => m.Value.Replace("\"", "")).ToList();
                foreach (var str in df)
                {
                    _dirFilter.Add(new Regex(string.Format("\\\\{0}\\\\", _sanitizeInput(str)), RegexOptions.Compiled | RegexOptions.IgnoreCase));
                }
            }

        }

        /// <summary>
        /// Escapes chars not to be used in regex
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string _sanitizeInput(string str)
        {
            return str.Replace(".", "\\.").Replace("$","\\$").Replace("*", ".*");
        }

        public bool MatchesFilter(string path)
        {
            if (path == null)
            {
                return false;
            }

            var filterFound = MatchesFile(path);

            if (filterFound)
            {
                return true;
            }

            return MatchesDirectory(path);
        }

        public bool MatchesFile(string path)
        {
            return _fileFilter.Any(r => r.IsMatch(path));   
        }

        public bool MatchesDirectory(string path)
        {
            return _dirFilter.Any(r => r.IsMatch(path));
        }
    }
}
