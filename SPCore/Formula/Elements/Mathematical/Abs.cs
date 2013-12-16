using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class Abs : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "ABS({Value})";
            }
        }

        /// <summary>
        /// Used to create: ABS({Value})
        /// </summary>
        public Abs(IValueType value)
        {
            this.Value = value;
        }        
    }
}
