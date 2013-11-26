using SPCore.Search.Linq;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search
{
    internal class FullTextSqlQueryBuilder: SearchQuery
    {
        public FullTextSqlQueryBuilder(ITranslatorFactory translatorFactory) : base(translatorFactory)
        {
        }
    }
}
