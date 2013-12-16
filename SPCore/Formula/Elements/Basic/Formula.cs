using System;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    class Formula : Element
    {
        [RequiredParameter]
        public string Text;

        protected override string Template
        {
            get
            {
                return "={Text}";
            }
        }

        /// <summary>
        /// Used to create: ={Text}
        /// </summary>
        public Formula(IValueType text)
        {
            this.Text = text.ToString();
        }

        /// <summary>
        /// Used to create: ={Text}
        /// </summary>
        public Formula(System.Linq.Expressions.Expression<Func<string>> expression)
        {
            this.Text = new Expression(expression).ToString();
        }
    }
}
