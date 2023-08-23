using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Errors.Interfaces;

namespace LanguageLib.Errors.Implementation
{
    public class LexicalError : IError
    {
        public string Message { get; set; }
        public int Position { get; set; }

        public LexicalError(string message, int position)
        {
            Message = message;
            Position = position;
        }

        public override string ToString() => $"{Message}. Позиция: {Position}";
    }
}
