
using System.Collections.Generic;

namespace SPCore.Caml.Interfaces
{
    interface IMultipleValueOperator<T>
    {
        IEnumerable<Value<T>> Values { get; set; }
    }
}
