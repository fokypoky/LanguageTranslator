﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageLib.AST.Interfaces;

namespace LanguageLib.AST.Implementation.Nodes.MathOperations
{
    public class MultiplyOperationASTNode : IMathOperationASTNode
    {
        public decimal Value { get; set; }
        public IASTNode LeftNode { get; set; }
        public IASTNode RightNode { get; set; }

        public MultiplyOperationASTNode(IASTNode leftNode, IASTNode rightNode)
        {
            LeftNode = leftNode;
            RightNode = rightNode;
        }

        public void Compute()
        {
            LeftNode.Compute();
            RightNode.Compute();

            Value = LeftNode.Value * RightNode.Value;
        }
    }
}
