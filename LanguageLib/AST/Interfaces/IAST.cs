﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageLib.AST.Interfaces
{
    public interface IAST
    {
        IRootASTNode Root { get; set; }
    }
}
