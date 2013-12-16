using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class WeekDay : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get
            {
                return "WEEKDAY({Value})";
            }
        }

        /// <summary>
        /// Used to create: WEEKDAY({Value})
        /// </summary>
        public WeekDay(IValueType value)
        {
            this.Value = value;
        }
    }
}
