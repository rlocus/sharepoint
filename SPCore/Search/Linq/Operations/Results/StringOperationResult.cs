using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Results
{
    internal class StringOperationResult : IOperationResult
    {
        private readonly string _result;

        public StringOperationResult(string result)
        {
            this._result = result;
        }
        
        public object Value
        {
            get { return this._result; }
        }

        public override string ToString()
        {
            return this._result;
        }
    }
}
