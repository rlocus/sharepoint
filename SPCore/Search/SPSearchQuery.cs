using SPCore.Search.Linq.Factories;
using SPCore.Search.Linq.Interfaces;
using System;

namespace SPCore.Search
{
    public class SPSearch
    {
        interface IOrderDirection
        {
            bool IsDefault();
        }

        public class OrderDirection : IOrderDirection
        {
            public static OrderDirection Default { get { return new None(); } }

            public static OrderDirection Convert(Type type)
            {
                if (type == typeof(Asc)) return new Asc();
                if (type == typeof(Desc)) return new Desc();

                return Default;
            }

            public bool IsDefault()
            {
                return this.GetType() == Default.GetType();
            }
        }

        // Marker class representing absence of order direction for "OrderBy" functionality
        public class None : OrderDirection { public override string ToString() { return string.Empty; } }
        // Marker class representing ASC order direction for "OrderBy" functionality
        public class Asc : OrderDirection { public override string ToString() { return "ASC"; } }
        // Marker class representing DESC order direction for "OrderBy" functionality
        public class Desc : OrderDirection { public override string ToString() { return "DESC"; } }

        private static readonly ITranslatorFactory TranslatorFactory;

        static SPSearch()
        {
            // factories setup
            var operandBuilder = new OperandBuilder();
            var operationResultBuilder = new OperationResultBuilder();
            var analyzerFactory = new AnalyzerFactory(operandBuilder, operationResultBuilder);
            TranslatorFactory = new TranslatorFactory(analyzerFactory);
        }

        public static ISearchQuery Query()
        {
            return new FullTextSqlQueryBuilder(TranslatorFactory);
        }
    }
}
