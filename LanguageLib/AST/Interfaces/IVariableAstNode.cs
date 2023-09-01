using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.AST.Interfaces
{
    public interface IVariableAstNode : IASTNode
    {
        string VariableName { get; set; }
    }
}
