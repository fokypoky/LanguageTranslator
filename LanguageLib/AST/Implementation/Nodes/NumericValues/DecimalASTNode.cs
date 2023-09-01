using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;

namespace LanguageLib.AST.Implementation.Nodes.NumericValues
{
    internal class DecimalASTNode : IASTNode
    {
        public decimal Value { get; set; }

        public DecimalASTNode(decimal value)
        {
            Value = value;
        }
        public void Compute() { }
    }
}
