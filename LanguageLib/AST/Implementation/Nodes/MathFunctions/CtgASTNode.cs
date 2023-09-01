using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;

namespace LanguageLib.AST.Implementation.Nodes.MathFunctions
{
    public class CtgASTNode : IMathFunctionASTNode
    {
        public decimal Value { get; set; }
        public IASTNode Argument { get; set; }

        public CtgASTNode(IASTNode argument)
        {
            Argument = argument;
        }
        
        public void Compute()
        {
            Argument.Compute();

            // ctg(x) = 1 / tg(x)
            Value = (decimal)1.0 / (decimal)Math.Tan((double)Argument.Value);
        }
    }
}
