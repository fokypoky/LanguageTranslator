using LanguageLib.Analyzers.Interfaces;
using LanguageLib.AST.Interfaces;

namespace LanguageLib.AST.Implementation.Nodes.Root
{
    public class RootASTNode : IRootASTNode
    {
        public decimal Value { get; set; } = 0;
        public List<IVariableAstNode> Variables { get; set; }

        public RootASTNode() { }

        public RootASTNode(List<IVariableAstNode> varables)
        {
            Variables = varables;
        }

        public void Compute()
        {
            foreach (var variable in Variables)
            {
                variable.Compute();
            }
        }
    }
}
