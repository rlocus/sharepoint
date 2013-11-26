
namespace SPCore.Search.Linq.Interfaces
{
    internal interface IOperationResultBuilder
    {
        IOperationResult CreateResult(string value);
        IOperationResult CreateResult(string[] values);
    }
}
