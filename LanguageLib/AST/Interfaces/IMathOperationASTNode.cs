using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.AST.Interfaces
{
    public interface IMathOperationASTNode : IASTNode
    {
        IASTNode LeftNode { get; set; }
        IASTNode RightNode { get; set; }
    }
}
