using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class Power : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType ValueLeft;

        [RequiredParameter]
        public IValueType ValueRight;

        protected override string Template
        {
            get 
            {
                return "POWER({ValueLeft}" + SectionSeparator + "{ValueRight})";
            }
        }

        /// <summary>
        /// Used to create: POWER({ValueLeft}, {ValueRight})
        /// </summary>
        public Power(IValueType valueLeft, IValueType valueRight)
        {
            this.ValueLeft = valueLeft;
            this.ValueRight = valueRight;
        }        
    }
}
