
namespace SPCore.Caml.Operators
{
    public sealed class And : NestedOperator
    {
        public And(params Operator[] operators)
            : base("And", operators)
        {
        }
    }
}
