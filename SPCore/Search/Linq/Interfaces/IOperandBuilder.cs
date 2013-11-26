using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    internal interface IOperandBuilder
    {
        IOperand CreateColumnOperand(Expression expr, IOperand valueOperand);
        IOperand CreateColumnOperandWithOrdering(Expression expr, SPSearch.OrderDirection orderDirection);
        IOperand CreateValueOperandForNativeSyntax(Expression expr);
        IOperand CreateValueOperandForNativeSyntax(Expression expr, Type explicitOperandType);
        IOperand CreateValueOperandForStringBasedSyntax(Expression expr);
    }
}
