using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;

namespace LanguageLib.AST.Implementation
{
    public class AST : IAST
    {
        public IRootASTNode Root { get; set; }

        public AST(IRootASTNode root)
        {
            Root = root;
        }
    }
}
