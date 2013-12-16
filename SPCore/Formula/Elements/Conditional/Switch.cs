using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;
using Expression = SPCore.Formula.Elements.Basic.Expression;

namespace SPCore.Formula.Elements.Conditional
{
    public class Switch : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public string Value;

        protected override string Template
        {
            get 
            {
                return "{Value}";
            }
        }

        /// <summary>
        /// Used to create: IF({Condition}, {CaseTrue}, IF({Condition}, {CaseTrue}, IF({Condition}, {CaseTrue}, {DefaultValue})))
        /// </summary>
        public Switch(IValueType defaultValue, params KeyValuePair<IConditionalType, IValueType>[] conditionCaseTruePair)
        {
            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < conditionCaseTruePair.Length; i++)
            {
                sb.AppendFormat("IF({0}" + SectionSeparator + "{1}" + SectionSeparator, conditionCaseTruePair[i].Key.ToString(), conditionCaseTruePair[i].Value.ToString());

                if ((i + 1) == conditionCaseTruePair.Length)
                {
                    sb.AppendFormat("{0}", defaultValue);
                    sb.Append(')', conditionCaseTruePair.Length);
                }
            }

            this.Value = sb.ToString();
        }

        /// <summary>
        /// Used to create: IF({Condition}, {CaseTrue}, IF({Condition}, {CaseTrue}, IF({Condition}, {CaseTrue}, {DefaultValue})))
        /// </summary>
        public Switch(IValueType defaultValue, params KeyValuePair<Expression<Func<string>>, IValueType>[] conditionCaseTruePair)
        {
            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < conditionCaseTruePair.Length; i++)
            {
                sb.AppendFormat("IF({0}" + SectionSeparator + "{1}" + SectionSeparator, new Expression(conditionCaseTruePair[i].Key), conditionCaseTruePair[i].Value);

                if ((i+1) == conditionCaseTruePair.Length)
                {
                    sb.AppendFormat("{0}", defaultValue);
                    sb.Append(')', conditionCaseTruePair.Length);
                }
            }

            this.Value = sb.ToString();
        }
    }
}
