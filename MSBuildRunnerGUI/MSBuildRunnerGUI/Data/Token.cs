using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSBuildRunnerGUI.Annotations;

namespace MSBuildRunnerGUI.Data
{
    public class Token
    {
        public List<string> Values { get; set; }

        public bool HasVariants => Values.Count > 1;

        /// <summary>
        /// Defines the unique name of the token without delimiters 
        /// </summary>
        public string TokenKey { get; set; }

        public bool IsActive { get; set; }

        public Token(bool isActive, params string[] values)
        {
            Values = new List<string>(values);
            if (Values.Any() == false)
            {
                throw new ArgumentNullException(nameof(values));
            }

            IsActive = isActive;
        }
    }
}
