using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class Round : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        [RequiredParameter]
        public IValueType DecimalPlaces;

        protected override string Template
        {
            get 
            {
                return "ROUND({Value}" + SectionSeparator + "{DecimalPlaces})";
            }
        }

        /// <summary>
        /// Used to create: ROUND({Value},{DecimalPlaces})
        /// </summary>
        public Round(IValueType value, IValueType decimalPlaces)
        {
            this.Value = value;
            this.DecimalPlaces = decimalPlaces;
        }        
    }
}
