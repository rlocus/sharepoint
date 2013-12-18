using System;
using System.Linq;
using System.Globalization;
using System.Linq.Expressions;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    internal sealed class Formula<T> : Element
        where T : Element, IValueType, IElementType
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
        public Formula(T text)
        {
            if (text == null) throw new ArgumentNullException("text");
            text.UseEnvironmentCulture = UseEnvironmentCulture;
            this.Text = text.ToString();
        }

        public Formula(T text, CultureInfo culture)
            : base(culture)
        {
            if (text == null) throw new ArgumentNullException("text");
            text.UseEnvironmentCulture = UseEnvironmentCulture;
            text.Culture = culture;
            this.Text = text.ToString();
        }

        /// <summary>
        /// Used to create: ={Text}
        /// </summary>
        public Formula(Expression<Func<string>> expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            this.Text = new Expression(expression) { UseEnvironmentCulture = UseEnvironmentCulture }.ToString();
        }

        public Formula(Expression<Func<string>> expression, CultureInfo culture)
            : base(culture)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            this.Text =
                new Expression(expression) { Culture = culture, UseEnvironmentCulture = UseEnvironmentCulture }.ToString();
        }
    }
}
