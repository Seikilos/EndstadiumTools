using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSBuildRunnerGUI.Data;

namespace MSBuildRunnerGUI.Logic
{
    public static class TokenParser
    {
        public static List<Token> Parse(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            var list = new List<Token>();

            var accum = new StringBuilder();
            var quoteOpened = false;

            foreach (var c in str)
            {

                if (c == '\"' || c == '\'')
                {
                    if (quoteOpened)
                    { 
                        quoteOpened = false;
                    }
                    else
                    {
                        quoteOpened = true;
                    }
                   
                  
                }

                if (c != ' ' || quoteOpened)
                {
                    accum.Append(c);
                }
                else
                {
                    list.Add(new Token(accum.ToString(), true));
                    accum.Clear();
                }
            }

            if (accum.Length != 0)
            {
                list.Add(new Token(accum.ToString(), true));
            }

            return list;
        }


    }
}
