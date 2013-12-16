using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class DateDiff : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType ValueLeft;

        [RequiredParameter]
        public IValueType ValueRight;

        [RequiredParameter]
        public IValueType Format;

        protected override string Template
        {
            get 
            {
                return "DATEDIF({ValueLeft}" + SPFormulaBuilder.SectionSeparator + "{ValueRight}" + SPFormulaBuilder.SectionSeparator + "{Format})";
            }
        }

        /// <summary>
        /// Used to create: DATEDIF({ValueLeft},{ValueRight},{Format})
        /// </summary>
        public DateDiff(IValueType valueLeft, IValueType valueRight, IValueType format)
        {
            this.ValueLeft = valueLeft;
            this.ValueRight = valueRight;
            this.Format = format;
        }        
    }
}
