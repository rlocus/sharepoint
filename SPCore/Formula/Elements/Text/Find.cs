using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Find : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType ValueLeft;

        [RequiredParameter]
        public IValueType ValueRight;

        protected override string Template
        {
            get 
            {
                return "FIND({ValueLeft}" + SectionSeparator + "{ValueRight})";
            }
        }

        /// <summary>
        /// Used to create: FIND({ValueLeft},{ValueRight})
        /// </summary>
        public Find(IValueType valueLeft, IValueType valueRight)
        {
            this.ValueLeft = valueLeft;
            this.ValueRight = valueRight;
        }        
    }
}
