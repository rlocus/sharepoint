using System;
using System.Linq;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class DateTimeValueOperand : ValueOperand<DateTime>
    {
        public bool IncludeTimeValue { get; set; }

        public DateTimeValueOperand(DateTime value, bool includeTimeValue) :
            base(typeof(SPManagedDataType.DateTime), value)
        {
            IncludeTimeValue = includeTimeValue;
        }

        public DateTimeValueOperand(string value, bool includeTimeValue) :
            this(value, includeTimeValue, false)
        {
        }

        public DateTimeValueOperand(string value, bool includeTimeValue, bool parseExact) :
            base(typeof(SPManagedDataType.DateTime), DateTime.MinValue)
        {
            IncludeTimeValue = includeTimeValue;

            // from re value come in sortable format ("s"), so we need to use ParseExact instead of Parse
            //if (parseExact)
            //{
            //    if (!string.IsNullOrEmpty(value) && value.EndsWith("Z"))
            //    {
            //        value = value.Substring(0, value.Length - 1);
            //    }
            //    else throw new InvalidValueForOperandTypeException(value, Type);
            //}
            //else throw new InvalidValueForOperandTypeException(value, Type);
        }


        public override string ToString()
        {
            //if (IncludeTimeValue)
            //{
               
            //}

            return string.Format("DATEADD (DAY, {0}, GETGMTDATE())", Convert.ToInt32((Value - DateTime.Now).TotalDays));
        }

        public override Expression ToExpression()
        {
            var expr = Expression.Constant(this.Value);

            if (this.IncludeTimeValue)
            {
                var mi =
                    ReflectionHelper.GetExtensionMethods(typeof(SPSearch).Assembly, typeof(DateTime)).FirstOrDefault(
                        m => m.Name == ReflectionHelper.IncludeTimeValue);
                return Expression.Call(mi, expr);
            }
            else
            {
                return expr;
            }
        }
    }
}
