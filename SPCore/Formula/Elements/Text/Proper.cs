using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Text
{
    public class Proper : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Value { get; set; }

        protected override string Template
        {
            get 
            {
                return "PROPER({Value})";
            }
        }

        /// <summary>
        /// Used to create: PROPER({Value})
        /// </summary>
        public Proper(IValueType value)
        {
            this.Value = value;
        }        
    }
}
