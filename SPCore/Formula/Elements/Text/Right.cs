using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Right : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType ValueLeft { get; set; }

        [RequiredParameter]
        public IValueType ValueRight;

        protected override string Template
        {
            get 
            {
                return "RIGHT({ValueLeft}" + SectionSeparator + "{ValueRight})";
            }
        }

        /// <summary>
        /// Used to create: RIGHT({ValueLeft},{ValueRight})
        /// </summary>
        public Right(IValueType valueLeft, IValueType valueRight)
        {
            this.ValueLeft = valueLeft;
            this.ValueRight = valueRight;
        }        
    }
}
