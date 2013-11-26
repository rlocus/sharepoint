using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operations.Results;

namespace SPCore.Search.Linq.Factories
{
    internal class OperationResultBuilder : IOperationResultBuilder
    {
        public IOperationResult CreateResult(string value)
        {
            return new StringOperationResult(value);
        }

        public IOperationResult CreateResult(string[] values)
        {
            return new StringArrayOperationResult(values);
        }
    }
}
