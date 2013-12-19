
using System.Xml.Linq;

namespace SPCore.Caml.Operators
{
    public abstract class Operator : QueryElement
    {
        protected Operator(string operatorName)
            : base(operatorName)
        {
        }

        protected Operator(string operatorName, string existingOperator)
            : base(operatorName, existingOperator)
        {
        }

        protected Operator(string operatorName, XElement existingOperator)
            : base(operatorName, existingOperator)
        {
        }

        public static Operator GetOperator(XElement existingOperator)
        {
            switch (existingOperator.Name.LocalName)
            {
                case "And":
                    return new And(existingOperator);
                case "Or":
                    return new Or(existingOperator);
                case "BeginsWith":
                    return new BeginsWith(existingOperator);
                case "Contains":
                    return new Contains(existingOperator);
                case "Eq":
                    return new Eq<object>(existingOperator);
                case "Geq":
                    return new Geq<object>(existingOperator);
                case "Gt":
                    return new Gt<object>(existingOperator);
                case "Leq":
                    return new Leq<object>(existingOperator);
                case "Lt":
                    return new Lt<object>(existingOperator);
                case "Neq":
                    return new Neq<object>(existingOperator);
                case "IsNull":
                    return new IsNull(existingOperator);
                case "IsNotNull":
                    return new IsNotNull(existingOperator);
                case "DateRangesOverlap":
                    return new DateRangesOverlap(existingOperator);
                default:
                    return null;
            }
        }
    }
}
