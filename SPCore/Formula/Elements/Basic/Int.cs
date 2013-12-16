using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class Int : ExtendedElement, IValueType, IConditionalType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "INT({Value})";
            }
        }

        /// <summary>
        /// Used to create: INT({Value})
        /// </summary>
        public Int(IValueType value)
        {
            this.Value = value;
        }        
    }
}
