using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class RoundDown : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        [RequiredParameter]
        public IValueType DecimalPlaces;

        protected override string Template
        {
            get 
            {
                return "ROUNDDOWN({Value}" + SectionSeparator + "{DecimalPlaces})";
            }
        }

        /// <summary>
        /// Used to create: ROUNDDOWN({Value},{DecimalPlaces})
        /// </summary>
        public RoundDown(IValueType value, IValueType decimalPlaces)
        {
            this.Value = value;
            this.DecimalPlaces = decimalPlaces;
        }        
    }
}
