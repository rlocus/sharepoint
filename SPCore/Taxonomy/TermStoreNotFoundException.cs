using System;

namespace SPCore.Taxonomy
{
    [Serializable]
    public class TermStoreNotFoundException : Exception
    {
        public TermStoreNotFoundException() { }
        public TermStoreNotFoundException(string message) : base(message) { }
        public TermStoreNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected TermStoreNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
