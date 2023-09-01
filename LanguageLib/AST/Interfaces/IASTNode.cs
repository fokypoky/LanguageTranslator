using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.AST.Interfaces
{
    public interface IASTNode
    {
        decimal Value { get; set; }
        void Compute();
    }
}
