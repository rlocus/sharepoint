using System;
using System.Linq.Expressions;

using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operands
{
    internal abstract class ValueOperand<T> : IOperand
    {
        protected T Value;
        protected Type Type;

        //public string TypeName
        //{
        //    get
        //    {
        //        return this.Type.Name;
        //    }
        //}

        protected ValueOperand(Type type, T value)
        {
            this.Type = type;
            this.Value = value;
        }
        
        public abstract Expression ToExpression();

        public object GetValue()
        {
           return Value;
        }
    }
}


