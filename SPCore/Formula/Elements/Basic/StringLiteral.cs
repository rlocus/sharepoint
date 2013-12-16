using System;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class StringLiteral : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public string Text;

        protected override string Template
        {
            get 
            {
                return "\"{Text}\"";
            }
        }

        /// <summary>
        /// Used to create: "{Text}"
        /// </summary>
        public StringLiteral(string text, params object[] parameters)
        {
            this.Text = String.Format(text, parameters);
        }        
    }
}
