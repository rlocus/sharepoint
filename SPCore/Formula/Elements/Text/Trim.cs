using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Trim : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "TRIM({Value})";
            }
        }

        /// <summary>
        /// Used to create: TRIM({Value})
        /// </summary>
        public Trim(IValueType value)
        {
            this.Value = value;
        }        
    }
}
