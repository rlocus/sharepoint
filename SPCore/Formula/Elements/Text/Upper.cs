using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Upper : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value;

        protected override string Template
        {
            get 
            {
                return "UPPER({Value})";
            }
        }

        /// <summary>
        /// Used to create: UPPER({Value})
        /// </summary>
        public Upper(IValueType value)
        {
            this.Value = value;
        }        
    }
}
