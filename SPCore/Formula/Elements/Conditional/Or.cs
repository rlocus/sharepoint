using System;
using System.Linq.Expressions;
using System.Text;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;
using Expression = SPCore.Formula.Elements.Basic.Expression;

namespace SPCore.Formula.Elements.Conditional
{
    public class Or : Element, IConditionalType, IValueType
    {
        [RequiredParameter]
        public string Conditions;

        protected override string Template
        {
            get 
            {
                return "OR({Conditions})";
            }
        }

        /// <summary>
        /// Used to create: OR({Condition1}, {Condition2}...)
        /// </summary>
        public Or(params IConditionalType[] conditions)
        {
            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < conditions.Length; i++)
            {
                sb.AppendFormat(conditions[i].ToString());

                if ((i + 1) < conditions.Length)
                {
                    sb.Append(SectionSeparator);
                }
            }

            this.Conditions = sb.ToString();
        }  
      
        /// <summary>
        /// Used to create: OR({Condition1}, {Condition2}...)
        /// </summary>
        public Or(params Expression<Func<string>>[] conditionalExpressions)
        {
            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < conditionalExpressions.Length; i++)
            {
                sb.AppendFormat(new Expression(conditionalExpressions[i]).ToString());

                if ((i + 1) < conditionalExpressions.Length)
                {
                    sb.Append(SectionSeparator);
                }
            }

            this.Conditions = sb.ToString();
        }
    }
}
