using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class PercentageLiteral : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "{Value}%";
            }
        }

        /// <summary>
        /// Used to create: "{Value}%"
        /// </summary>
        public PercentageLiteral(IValueType value)
        {
            this.Value = value;
        }        
    }
}
