using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Lower : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "LOWER({Value})";
            }
        }

        /// <summary>
        /// Used to create: LOWER({Value})
        /// </summary>
        public Lower(IValueType value)
        {
            this.Value = value;
        }        
    }
}
