using System;
using System.Collections.Generic;
using System.Linq;
using SPCore.Caml.Clauses;
using SPCore.Caml.Interfaces;
using SPCore.Caml.Operators;

namespace SPCore.Caml
{
    public static class Extensions
    {
        public static Where CombineAnd<T>(this Where where, T op)
           where T : Operator, IFieldOperator
        {
            where.And(op);
            return where;
        }

        public static Where CombineOr<T>(this Where where, T op)
           where T : Operator, IFieldOperator
        {
            where.Or(op);
            return where;
        }

        public static OrderBy ThenBy(this OrderBy orderBy, Guid fieldId)
        {
            return orderBy.ThenBy(fieldId, false);
        }

        public static OrderBy ThenBy(this OrderBy orderBy, Guid fieldId, bool ascending)
        {
            var fields = orderBy.FieldRefs.ToList();
            fields.Add(new FieldRef() { FieldId = fieldId, Ascending = ascending });
            orderBy.FieldRefs = fields;
            return orderBy;
        }

        public static OrderBy ThenBy(this OrderBy orderBy, string fieldName)
        {
            return orderBy.ThenBy(fieldName, null);
        }

        public static OrderBy ThenBy(this OrderBy orderBy, string fieldName, bool? ascending)
        {
            List<FieldRef> fields = orderBy.FieldRefs.ToList();
            fields.Add(new FieldRef() { Name = fieldName, Ascending = ascending });
            orderBy.FieldRefs = fields;
            return orderBy;
        }

        public static OrderBy ThenBy(this OrderBy orderBy, FieldRef fieldRef)
        {
            List<FieldRef> fields = orderBy.FieldRefs.ToList();
            fields.Add(fieldRef);
            orderBy.FieldRefs = fields;
            return orderBy;
        }

        public static GroupBy ThenBy(this GroupBy groupBy, Guid fieldId)
        {
            return groupBy.ThenBy(fieldId, false);
        }

        public static GroupBy ThenBy(this GroupBy groupBy, Guid fieldId, bool collapsed)
        {
            List<FieldRef> fields = groupBy.FieldRefs.ToList();
            fields.Add(new FieldRef() { FieldId = fieldId, Ascending = false });
            groupBy.FieldRefs = fields;
            return groupBy;
        }

        public static GroupBy ThenBy(this GroupBy groupBy, string fieldName)
        {
            return groupBy.ThenBy(fieldName, false);
        }

        public static GroupBy ThenBy(this GroupBy groupBy, string fieldName, bool collapsed)
        {
            List<FieldRef> fields = groupBy.FieldRefs.ToList();
            fields.Add(new FieldRef() { Name = fieldName, Ascending = false });
            groupBy.FieldRefs = fields;
            return groupBy;
        }

        public static GroupBy ThenBy(this GroupBy groupBy, FieldRef fieldRef)
        {
            List<FieldRef> fields = groupBy.FieldRefs.ToList();
            fields.Add(fieldRef);
            groupBy.FieldRefs = fields;
            return groupBy;
        }
    }
}
