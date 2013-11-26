using SPCore.Search.Linq.Interfaces;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class ColumnOperand : IOperand
    {
        private readonly List<KeyValuePair<string, string>> _attributes;

        public string ColumnName { get; protected set; }
        
        public List<KeyValuePair<string, string>> Attributes
        {
            get
            {
                return this._attributes;
            }
        }

        protected ColumnOperand(List<KeyValuePair<string, string>> attributes)
        {
            this._attributes = attributes;
        }

        public ColumnOperand(string columnName)
        {
            this.Initialize(columnName);
        }

        public ColumnOperand(string columnName, List<KeyValuePair<string, string>> attributes) :
            this (attributes)
        {
            this.Initialize(columnName);
        }
        
        protected void Initialize(string columnName)
        {
            this.ColumnName = columnName;
        }

        public override string ToString()
        {
            return ColumnName;
        }

        public virtual Expression ToExpression()
        {
            if (!string.IsNullOrEmpty(this.ColumnName))
            {
                var mi = typeof(NameValueCollection).GetProperty(ReflectionHelper.Item, typeof(object), new[] { typeof(string) }, null).GetGetMethod();
                return
                    Expression.Call(
                        Expression.Parameter(typeof(NameValueCollection), ReflectionHelper.CommonParameterName),
                        mi, new[] { Expression.Constant(this.ColumnName) });
            }

            throw new ColumnOperandShouldContainNameException();
        }

        public object GetValue()
        {
            return ColumnName;
        }
    }
}


