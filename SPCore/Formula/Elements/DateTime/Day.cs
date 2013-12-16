using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class Day : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "DAY({Value})";
            }
        }

        /// <summary>
        /// Used to create: DAY({Value})
        /// </summary>
        public Day(IValueType value)
        {
            this.Value = value;
        }        
    }
}
