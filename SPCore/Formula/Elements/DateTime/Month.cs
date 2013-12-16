using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class Month : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "MONTH({Value})";
            }
        }

        /// <summary>
        /// Used to create: MONTH({Value})
        /// </summary>
        public Month(IValueType value)
        {
            this.Value = value;
        }        
    }
}
