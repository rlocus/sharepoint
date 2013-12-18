using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Rept : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType ValueLeft { get; set; }

        [RequiredParameter]
        public IValueType ValueRight;

        protected override string Template
        {
            get 
            {
                return "REPT({ValueLeft}" + SectionSeparator + "{ValueRight})";
            }
        }

        /// <summary>
        /// Used to create: REPT({ValueLeft},{ValueRight})
        /// </summary>
        public Rept(IValueType valueLeft, IValueType valueRight)
        {
            this.ValueLeft = valueLeft;
            this.ValueRight = valueRight;
        }        
    }
}
