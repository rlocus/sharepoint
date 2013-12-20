using System;
using System.Linq;
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

        protected SingleFieldOperator(string operatorName, string existingSingleFieldOperator)
            : base(operatorName, existingSingleFieldOperator)
        {
        }

        protected SingleFieldOperator(string operatorName, XElement existingSingleFieldOperator)
            : base(operatorName, existingSingleFieldOperator)
        {
        }

        protected override void OnParsing(XElement existingSingleFieldValueOperator)
        {
            XElement existingFieldRef = existingSingleFieldValueOperator.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "FieldRef", StringComparison.InvariantCultureIgnoreCase));

            if (existingFieldRef != null)
            {
                FieldRef = new FieldRef(existingFieldRef);
            }
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            el.AddFirst(FieldRef.ToXElement());
            return el;
        }
    }
}
