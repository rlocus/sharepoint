using System.Text;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Mathematical
{
    public class Min : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public string Value;

        protected override string Template
        {
            get 
            {
                return "MIN({Value})";
            }
        }

        /// <summary>
        /// Used to create: MIN({Value})
        /// </summary>
        public Min(params IValueType[] value)
        {
            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < value.Length; i++)
            {
                sb.Append(value[i].ToString());

                if ((i + 1) < value.Length)
                {
                    sb.Append(SectionSeparator);
                }
            }

            this.Value = sb.ToString();
        }        
    }
}
