using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class Year : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "YEAR({Value})";
            }
        }

        /// <summary>
        /// Used to create: YEAR({Value})
        /// </summary>
        public Year(IValueType value)
        {
            this.Value = value;
        }        
    }
}
