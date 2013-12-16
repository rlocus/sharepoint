using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class Hour : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "HOUR({Value})";
            }
        }

        /// <summary>
        /// Used to create: HOUR({Value})
        /// </summary>
        public Hour(IValueType value)
        {
            this.Value = value;
        }        
    }
}
