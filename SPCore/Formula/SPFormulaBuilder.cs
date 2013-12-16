using System;
using System.Globalization;
using System.Linq.Expressions;
using SPCore.Formula.Base.Interfaces;
using Basic = SPCore.Formula.Elements.Basic;

namespace SPCore.Formula
{
    public class SPFormulaBuilder
    {
        /// <summary>
        /// Do not allow to create instances of this type
        /// </summary>
        private SPFormulaBuilder()
        {
        }

        /// <summary>
        /// Used to create: "={Text}"
        /// Note: Formula class cannot be used directly. Instead of instantiating Formula class, use this method.
        /// </summary>
        public static string CreateFormula(IValueType element, bool useEnvironmentCulture = false)
        {
            return new Basic.Formula(element) { UseEnvironmentCulture = useEnvironmentCulture }.ToString();
        }

        public static string CreateFormula(IValueType element, CultureInfo culture)
        {
            return new Basic.Formula(element, culture).ToString();
        }

        /// <summary>
        /// Used to create: "={Text}"
        /// Note: Formula class cannot be used directly. Instead of instantiating Formula class, use this method.
        /// </summary>
        public static string CreateFormula(Expression<Func<string>> expression, bool useEnvironmentCulture = false)
        {
            return new Basic.Formula(expression) { UseEnvironmentCulture = useEnvironmentCulture }.ToString();
        }

        public static string CreateFormula(Expression<Func<string>> expression, CultureInfo culture)
        {
            return new Basic.Formula(expression, culture).ToString();
        }
    }
}
