using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class Len : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "LEN({Value})";
            }
        }

        /// <summary>
        /// Used to create: LEN({Value})
        /// </summary>
        public Len(IValueType value)
        {
            this.Value = value;
        }        
    }
}
