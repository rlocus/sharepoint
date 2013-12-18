
using System.Collections.Generic;

namespace SPCore.Caml.Interfaces
{
    public interface IMultiFieldOperator : IFieldOperator
    {
        IEnumerable<FieldRef> FieldRefs { get; set; }
    }
}
