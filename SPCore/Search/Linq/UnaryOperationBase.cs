using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq
{
    internal abstract class UnaryOperationBase : OperationBase
    {
        protected IOperand ColumnOperand;

        protected UnaryOperationBase(IOperationResultBuilder operationResultBuilder,
            IOperand columnOperand) :
            base(operationResultBuilder)
        {
            this.ColumnOperand = columnOperand;
        }
    }
}
