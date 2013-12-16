using System;
using System.Globalization;
using System.Linq.Expressions;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class Formula : Element
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

        public Formula(IValueType text, CultureInfo culture)
            : base(culture)
        {
            this.Text = text.ToString();
        }

        /// <summary>
        /// Used to create: ={Text}
        /// </summary>
        public Formula(Expression<Func<string>> expression)
        {
            this.Text = new Expression(expression).ToString();
        }

        public Formula(Expression<Func<string>> expression, CultureInfo culture)
            : base(culture)
        {
            this.Text = new Expression(expression).ToString();
        }
    }
}
