
namespace SPCore.Search
{
    public class BaseFieldType
    {
        public static explicit operator BaseFieldType(string s) { return null; }
    }

    public class BaseFieldTypeWithOperators : BaseFieldType
    {
        public static bool operator <(object c1, BaseFieldTypeWithOperators c2) { return false; }
        public static bool operator >(object c1, BaseFieldTypeWithOperators c2) { return false; }
        public static bool operator <=(object c1, BaseFieldTypeWithOperators c2) { return false; }
        public static bool operator >=(object c1, BaseFieldTypeWithOperators c2) { return false; }
    }

    public class StringBasedFieldType : BaseFieldTypeWithOperators
    {
        public bool Contains(string text) { return true; }
        public bool StartsWith(string text) { return true; }
    }

    public static class SPManagedDataType
    {
        public class Text : StringBasedFieldType { }
        public class Integer : BaseFieldTypeWithOperators { }
        public class Double : BaseFieldTypeWithOperators { }
        public class Decimal : BaseFieldTypeWithOperators { }
        public class Boolean : BaseFieldType { }
        public class Binary : BaseFieldType { }
        public class DateTime : BaseFieldTypeWithOperators
        {
            public DateTime IncludeTimeValue() { return this; }
        }

    }
}
