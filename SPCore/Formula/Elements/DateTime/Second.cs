using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class Second : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "SECOND({Value})";
            }
        }

        /// <summary>
        /// Used to create: SECOND({Value})
        /// </summary>
        public Second(IValueType value)
        {
            this.Value = value;
        }        
    }
}
