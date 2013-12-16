
namespace SPCore.Caml.Operators
{
    public abstract class Operator : QueryElement
    {
        protected Operator(string operatorName)
            : base(operatorName)
        {
        }
    }
}
