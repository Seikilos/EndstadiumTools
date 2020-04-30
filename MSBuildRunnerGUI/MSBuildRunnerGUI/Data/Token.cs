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
        public string Value { get; set; }

        public bool IsActive { get; set; }

        public Token([NotNull] string value, bool isActive)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsActive = isActive;
        }
    }
}
