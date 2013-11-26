using System;
using System.Collections;
using Microsoft.SharePoint;

namespace SPCore.Linq
{
    public sealed class EntityListFieldInfo
    {
        public Guid Id { get; set; }
        public string InternalName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public SPFieldType FieldType { get; set; }
        public bool AllowMultipleValues { get; set; }
        public IEnumerable Choices { get; set; }
        public bool FillInChoice { get; set; }
        public bool Hidden { get; set; }
        public bool IsCalculated { get; set; }
        public string LookupDisplayColumn { get; set; }
        public string LookupList { get; set; }
        public string PrimaryFieldId { get; set; }
        public bool ReadOnlyField { get; set; }
        public bool Required { get; set; }
    }
}
