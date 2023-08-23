using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.Errors.Interfaces
{
    public interface IError
    {
        string Message { get; set; }
        int Position { get; set; }
    }
}
