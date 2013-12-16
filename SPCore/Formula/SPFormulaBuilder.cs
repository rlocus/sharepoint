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
        /// Exception is being thrown if total characters count of 
        /// calculated field formula is bigger than this property value.
        /// </summary>
        private static int _MAXIMUM_FORMULA_LENGTH = 1024;
        public static int MAXIMUM_FORMULA_LENGTH
        {
            get
            {
                return _MAXIMUM_FORMULA_LENGTH;
            }

            protected set
            {
                _MAXIMUM_FORMULA_LENGTH = value;
            }
        }

        /// <summary>
        /// Whether to use environments current culture or not.
        /// 
        /// If this property is set to false, InvariantCulture is used. 
        /// Otherwise environments culture is used.
        /// </summary>
        public static bool UseEnvironmentCulture;

        /// <summary>
        /// By setting Culture property, you override environments CurrentCulture
        /// </summary>
        private static CultureInfo _culture;
        public static CultureInfo Culture
        {
            get
            {
                if (_culture == null)
                {
                    return UseEnvironmentCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
                }

                return _culture;
            }

            set
            {
                _culture = value;
            }
        }

        /// <summary>
        /// Current NumberFormatInfo that depends on Culture property value
        /// </summary>
        public static NumberFormatInfo NumberFormat
        {
            get
            {
                return Culture.NumberFormat;
            }
        }

        /// <summary>
        /// Element sections separator character that depends on current culture.
        /// </summary>
        public static string SectionSeparator
        {
            get
            {
                return NumberFormat.NumberDecimalSeparator == "," ? ";" : ",";
            }
        }

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
        public static string CreateFormula(IValueType element)
        {
            return new Basic.Formula(element).ToString();
        }

        /// <summary>
        /// Used to create: "={Text}"
        /// Note: Formula class cannot be used directly. Instead of instantiating Formula class, use this method.
        /// </summary>
        public static string CreateFormula(Expression<Func<string>> expression)
        {
            return new Basic.Formula(expression).ToString();
        }
    }
}
