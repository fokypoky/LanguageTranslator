namespace LanguageLib.AST.Interfaces
{
    public interface IRootASTNode : IASTNode
    {
        List<IVariableAstNode> Variables { get; set; }
    }
}
