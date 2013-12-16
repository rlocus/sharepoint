using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class DateValue : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get
            {
                return "DATEVALUE({Value})";
            }
        }

        /// <summary>
        /// Used to create: DATEVALUE({Value})
        /// </summary>
        public DateValue(IValueType value)
        {
            this.Value = value;
        }
    }
}
