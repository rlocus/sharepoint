using System;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class ColumnReference : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public string Name;

        protected override string Template
        {
            get 
            {
                return "{Name}";
            }
        }

        /// <summary>
        /// Used to create: [{Name}]
        /// </summary>
        public ColumnReference(string name)
        {
            this.Name = name.Contains(" ") ? String.Format("[{0}]", name) : name;
        }
    }
}
