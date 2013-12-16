using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class Even : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "EVEN({Value})";
            }
        }

        /// <summary>
        /// Used to create: EVEN({Value})
        /// </summary>
        public Even(IValueType value)
        {
            this.Value = value;
        }        
    }
}
