using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.Errors.Interfaces;

namespace LanguageLib.Errors
{
    public class SyntacticalError : IError
    {
        public string Message { get; set; }
        public int Position { get; set; }

        public SyntacticalError(string message, int position)
        {
            Message = message;
            Position = position;
        }

        public override string ToString() => $"{Message}. Позиция: {Position}";
    }
}
