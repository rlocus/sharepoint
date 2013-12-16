using System;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class ConstantLiteral : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public string Text;

        protected override string Template
        {
            get 
            {
                return "{Text}";
            }
        }

        /// <summary>
        /// Used to create: {Text}
        /// </summary>
        public ConstantLiteral(string text, params object[] parameters)
        {
            this.Text = String.Format(text, parameters);
        }        
    }
}
