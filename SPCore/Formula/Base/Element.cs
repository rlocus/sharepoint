using System;
using System.Globalization;
using System.Reflection;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Base
{
    /// <summary>
    /// Abstract element class. Each element type must inherit from this class.
    /// </summary>
    public abstract class Element : IElementType
    {
        /// <summary>
        /// Exception is being thrown if total characters count of 
        /// calculated field formula is bigger than this property value.
        /// </summary>
        private const int _MAXIMUM_FORMULA_LENGTH = 1024;

        protected int MaximumFormulaLength
        {
            get
            {
                return _MAXIMUM_FORMULA_LENGTH;
            }
        }

        /// <summary>
        /// Whether to use environments current culture or not.
        /// 
        /// If this property is set to false, InvariantCulture is used. 
        /// Otherwise environments culture is used.
        /// </summary>
        public bool UseEnvironmentCulture { get; set; }

    

        /// <summary>
        /// By setting Culture property, you override environments CurrentCulture
        /// </summary>
        private CultureInfo _culture;

        protected Element()
        {
        }

        protected Element(CultureInfo culture)
        {
            _culture = culture;
        }

        public CultureInfo Culture
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
        protected NumberFormatInfo NumberFormat
        {
            get
            {
                return Culture.NumberFormat;
            }
        }

        /// <summary>
        /// Element sections separator character that depends on current culture.
        /// </summary>
        protected string SectionSeparator
        {
            get
            {
                return NumberFormat.NumberDecimalSeparator == "," ? ";" : ",";
            }
        }

        /// <summary>
        /// Element template. Can contain {Property} or {Field}.
        /// </summary>
        protected abstract string Template
        {
            get;
        }

        private static bool FilterMembers(MemberInfo m, object filterCriteria)
        {
            return ((m.GetCustomAttributes(typeof(RequiredParameter), false).Length == 1) || (m.GetCustomAttributes(typeof(OptionalParameter), false).Length == 1));
        }

        // TODO: rewrite this method in following fashion: with regex search for {...}, than search for Fields or Properties with ... name, if they are required check value for null
        /// <summary>
        /// Returns formatted formula. If formula is longer than maximum allowed length InvalidOperationException is thrown.
        /// </summary>
        /// <returns>String representation of formula.</returns>
        public sealed override string ToString()
        {
            string formula = this.Template;

            MemberInfo[] memberInfos = this.GetType().FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, FilterMembers, null);

            foreach (MemberInfo memberInfo in memberInfos)
            {
                object[] requiredAttrs = memberInfo.GetCustomAttributes(typeof(RequiredParameter), false);
                object[] optionalAttrs = memberInfo.GetCustomAttributes(typeof(OptionalParameter), false);

                object fieldValue = null;

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        {
                            fieldValue = (memberInfo as FieldInfo).GetValue(this);
                            break;
                        }

                    case MemberTypes.Property:
                        {
                            fieldValue = (memberInfo as PropertyInfo).GetValue(this, null);
                            break;
                        }
                }

                // if is required and null throw exception
                if (requiredAttrs.Length == 1 && fieldValue == null)
                {
                    throw new ArgumentException(String.Format("Field {0} is required and must not be null.", memberInfo.Name));
                }
                if (requiredAttrs.Length == 1 && fieldValue != null)
                {
                    RequiredParameter attr = requiredAttrs[0] as RequiredParameter;

                    // if parameter has overriden Name use it, otherwise use name of field
                    formula = attr.Name != null
                        ? formula.Replace("{" + attr.Name + "}", fieldValue.ToString())
                        : formula.Replace("{" + memberInfo.Name + "}", fieldValue.ToString());
                }
                else if (optionalAttrs.Length == 1 && fieldValue != null)
                {
                    OptionalParameter attr = optionalAttrs[0] as OptionalParameter;

                    // if parameter has overriden Name use it, otherwise use name of field
                    formula = attr.Name != null
                        ? formula.Replace("{" + attr.Name + "}", fieldValue.ToString())
                        : formula.Replace("{" + memberInfo.Name + "}", fieldValue.ToString());
                }
                else if (optionalAttrs.Length == 1 && fieldValue == null)
                {
                    OptionalParameter attr = optionalAttrs[0] as OptionalParameter;

                    // if parameter has overriden Name use it, otherwise use name of field
                    formula = attr.Name != null
                        ? formula.Replace("{" + attr.Name + "}", "")
                        : formula.Replace("{" + memberInfo.Name + "}", "");
                }
            }

            if (formula.Length > MaximumFormulaLength)
            {
                throw new InvalidOperationException(String.Format("This formula contains {0} characters which is more than maximum allowed length.", formula.Length));
            }

            return formula;
        }
    }
}
