using System;
using System.Linq.Expressions;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class Group : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "({Value})";
            }
        }

        /// <summary>
        /// Used to create: ({Value})
        /// </summary>
        public Group(IValueType value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Used to create: ({Value})
        /// </summary>
        public Group(Expression<Func<string>> expression)
        {
            this.Value = new Expression(expression);
        }
    }
}
