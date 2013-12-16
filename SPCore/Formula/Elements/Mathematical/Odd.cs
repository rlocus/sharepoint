using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class Odd : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "ODD({Value})";
            }
        }

        /// <summary>
        /// Used to create: ODD({Value})
        /// </summary>
        public Odd(IValueType value)
        {
            this.Value = value;
        }        
    }
}
