using System.Xml.Linq;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class SingleFieldOperator : Operator, IFieldOperator
    {
        public FieldRef FieldRef { get; set; }

        protected SingleFieldOperator(string operatorName, FieldRef fieldRef)
            : base(operatorName)
        {
            FieldRef = fieldRef;
        }

        public override XElement ToXElement()
        {
            var ele = base.ToXElement();
            ele.Add(FieldRef.ToXElement());
            return ele;
        }
    }
}
