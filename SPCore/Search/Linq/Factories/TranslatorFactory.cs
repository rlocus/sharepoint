using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Factories
{
    internal class TranslatorFactory : ITranslatorFactory
    {
        private readonly IAnalyzerFactory _analyzerFactory;

        public TranslatorFactory(IAnalyzerFactory analyzerFactory)
        {
            this._analyzerFactory = analyzerFactory;
        }

        public ITranslator Create(LambdaExpression expr)
        {
            var analyzer = _analyzerFactory.Create(expr);
            return new GenericTranslator(analyzer);
        }
    }
}