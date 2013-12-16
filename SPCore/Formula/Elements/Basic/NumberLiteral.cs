using System;
using System.Globalization;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class NumberLiteral<T> : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public string Number;

        protected override string Template
        {
            get 
            {
                return "{Number}";
            }
        }

        /// <summary>
        /// Used to create: {Number}
        /// </summary>
        public NumberLiteral(T number)
        {
            CultureInfo ci = new CultureInfo("en-US");

            if (typeof(T) == typeof(Double))
            {
                // TODO: set regional settings
                this.Number = Convert.ToDouble(number, ci).ToString(ci);
            }
            else if (typeof(T) == typeof(Decimal))
            {
                this.Number = Convert.ToDecimal(number, ci).ToString(ci);
            }
            else
            {
                throw new InvalidOperationException(number.GetType().FullName + " is unknown type.");
            }
        }  
    }
}
