using System;

namespace SPCore.Formula.Base.Attributes
{
    /// <summary>
    /// Formula parameter marker attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredParameter : Attribute
    {
        public string Name
        {
            get;
            set;
        }
    }
}
