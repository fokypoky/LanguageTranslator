using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;

namespace LanguageLib.AST.Implementation.Nodes.MathFunctions
{
    public class CosASTNode : IMathFunctionASTNode
    {
        public decimal Value { get; set; }
        public IASTNode Argument { get; set; }

        public CosASTNode(IASTNode argument)
        {
            Argument = argument;
        }

        public void Compute()
        {
            Argument.Compute();

            Value = (decimal) Math.Cos((double)Argument.Value);
        }

    }
}
