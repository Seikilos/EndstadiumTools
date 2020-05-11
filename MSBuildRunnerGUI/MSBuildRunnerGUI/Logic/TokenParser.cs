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
        private static char[] ArgBeginChars = {'-','/'};
        
        /// <summary>
        /// This delimiter divides a sub property
        /// </summary>
        private static char SubDelimiter = ':';

        /// <summary>
        /// This char is an assignment only when there was a SubDelimiter first
        /// </summary>
        private static char SubAssignment = '=';


        /// <summary>
        /// These items should not be grouped if only SubDelimiter and no SubAssignment was set
        /// </summary>
        private static string[] ForceNotGroupedNames = {"p", "property"};

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
                    // Append only if something was accumulated
                    if (accum.Length > 0)
                    {
                        list.Add(new Token(true, accum.ToString()));
                        accum.Clear();
                    }
                }
            }

            if (accum.Length != 0)
            {
                list.Add(new Token(true, accum.ToString() ));
            }

            return GroupTokens(list);
        }

        private static List<Token> GroupTokens(List<Token> original)
        {
            var dict = new Dictionary<string, Token>();

            foreach (var token in original)
            {
                var tokenKey = GetTokenKey(token.Values[0]);
                token.TokenKey = tokenKey;

                if (dict.ContainsKey(tokenKey) == false)
                {
                    dict[tokenKey] = token;
                }
                else
                {
                    // Merge this
                    dict[tokenKey].Values.Add(token.Values[0]);
                }
            }

            return dict.Select(kv => kv.Value).ToList();
        }

        private static string GetTokenKey(string tokenValue)
        {
            var trimmed = tokenValue.TrimStart(ArgBeginChars);

            if (trimmed.Contains(SubDelimiter))
            {
                // something:foo=bar, use only something:foo then
                if (trimmed.Contains(SubAssignment))
                {
                    // Take everything up to the assignment
                    return trimmed.Substring(0, trimmed.IndexOf(SubAssignment));
                }

                // something:foo, use only something then
                // unless the first part is in the ForceNotGroupedNames list, then pick the entire string, usually for /p:A /p:B
                var firstPart = trimmed.Substring(0, trimmed.IndexOf(SubDelimiter));
                if (ForceNotGroupedNames.Any(s => s == firstPart))
                {
                    // Found special one, return full
                    return trimmed;
                }

                return trimmed.Substring(0, trimmed.IndexOf(SubDelimiter));
            }

            // Return full otherwise, this is the entire token name
            return trimmed; 
        }
    }
}
