using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class Minute : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "MINUTE({Value})";
            }
        }

        /// <summary>
        /// Used to create: MINUTE({Value})
        /// </summary>
        public Minute(IValueType value)
        {
            this.Value = value;
        }        
    }
}
