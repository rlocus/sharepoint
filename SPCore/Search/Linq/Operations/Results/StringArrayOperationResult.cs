using SPCore.Search.Linq.Interfaces;
using System.Text;

namespace SPCore.Search.Linq.Operations.Results
{
    internal class StringArrayOperationResult : IOperationResult
    {
        private readonly string[] _results;

        public StringArrayOperationResult(string[] results)
        {
            this._results = results;
        }

        public object Value
        {
            get { return this._results; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < _results.Length; i++)
            {
                string result = _results[i];

                if (i > 0) { sb.Append(", "); }

                sb.Append(result);
            }
            return sb.ToString();
        }
    }
}
