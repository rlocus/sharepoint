using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Left : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType ValueLeft;

        [RequiredParameter]
        public IValueType ValueRight;

        protected override string Template
        {
            get 
            {
                return "LEFT({ValueLeft}" + SectionSeparator + "{ValueRight})";
            }
        }

        /// <summary>
        /// Used to create: LEFT({ValueLeft},{ValueRight})
        /// </summary>
        public Left(IValueType valueLeft, IValueType valueRight)
        {
            this.ValueLeft = valueLeft;
            this.ValueRight = valueRight;
        }        
    }
}
