
namespace SPCore.Caml.Operators
{
    public sealed class Or : NestedOperator
    {
        public Or(params Operator[] operators)
            : base("Or", operators)
        {
        }
    }
}
