using System.Xml.Linq;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class SingleFieldOperator : Operator, ISingleFieldOperator
    {
        public FieldRef FieldRef { get; set; }

        protected SingleFieldOperator(string operatorName, FieldRef fieldRef)
            : base(operatorName)
        {
            FieldRef = fieldRef;
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            el.Add(FieldRef.ToXElement());
            return el;
        }
    }
}
