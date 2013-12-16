using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class Text : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        [RequiredParameter]
        public IValueType Format;

        protected override string Template
        {
            get 
            {
                return "TEXT({Value}" + SPFormulaBuilder.SectionSeparator + "{Format})";
            }
        }

        /// <summary>
        /// Used to create: TEXT({Value},{Format})
        /// </summary>
        public Text(IValueType value, IValueType format)
        {
            this.Value = value;
            this.Format = format;
        }        
    }
}
