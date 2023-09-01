using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;

namespace LanguageLib.AST.Implementation.Nodes.Variables
{
    public class VariableASTNode : IVariableAstNode
    {
        public decimal Value { get; set; }
        public string VariableName { get; set; }

        public VariableASTNode(decimal value, string variableName)
        {
            Value = value;
            VariableName = variableName;
        }

        public VariableASTNode(string variableName)
        {
            VariableName = variableName;
        }
        
        public void Compute() { }
    }
}
