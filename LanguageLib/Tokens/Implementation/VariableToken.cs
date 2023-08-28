using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LanguageLib.Tokens.Implementation.Enums;
using LanguageLib.Tokens.Interfaces;

namespace LanguageLib.Tokens.Implementation
{
    public class VariableToken : IToken, IVariableToken
    {
        public string Name { get; set; }
        public TokenType Type { get; set; } = TokenType.Variable;
        public string Value { get; set; } = "";

        public string Regex { get; set; } = @"[a-zA-Z][a-zA-Z0-9]*"; 
        /*@"^[A-Z|a-z][A-Z|a-z|0-9]*$";*/
        public int Position { get; set; } = -1;

        public VariableToken()
        {
        }

        private VariableToken(string value, int position, string name)
        {
            Value = value;
            Position = position;
            Name = name;
        }

        public bool IsMatch(string word) => System.Text.RegularExpressions.Regex.IsMatch(word, Regex);
        
        public IToken GetTokenObject(string value, int position = 0)
        {
            return new VariableToken(value, position, value);
        }
    }
}
