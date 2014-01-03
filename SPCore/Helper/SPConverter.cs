using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace SPCore.Helper
{
    public static class SPConverter
    {
        public static Type GetValueType(SPFieldType type, bool allowMultipleValues = false)
        {
            if (type == SPFieldType.Guid)
            {
                return typeof(Guid);
            }
            if (type == SPFieldType.Text || type == SPFieldType.Note || type == SPFieldType.Choice)
            {
                return typeof(string);
            }
            if (type == SPFieldType.Number || type == SPFieldType.Currency)
            {
                return typeof(double);
            }
            if (type == SPFieldType.MultiChoice)
            {
                return typeof(SPFieldMultiChoiceValue);
            }
            if (type == SPFieldType.Boolean || type == SPFieldType.Recurrence || type == SPFieldType.Attachments || type == SPFieldType.AllDayEvent || type == SPFieldType.CrossProjectLink)
            {
                return typeof(bool);
            }
            if (type == SPFieldType.Lookup)
            {
                return allowMultipleValues ? typeof(SPFieldLookupValueCollection) : typeof(SPFieldLookupValue);
            }
            if (type == SPFieldType.User)
            {
                return allowMultipleValues ? typeof(SPFieldUserValueCollection) : typeof(SPFieldUserValue);
            }
            if (type == SPFieldType.URL)
            {
                return typeof(SPFieldUrlValue);
            }
            if (type == SPFieldType.DateTime)
            {
                return typeof(DateTime);
            }
            if (type == SPFieldType.Integer || type == SPFieldType.Counter || type == SPFieldType.ModStat || type == SPFieldType.WorkflowStatus)
            {
                return typeof(int);
            }
            if (type == SPFieldType.ContentTypeId)
            {
                return typeof(SPContentTypeId);
            }

            return null;
        }

        public static object ConvertValue(SPFieldType type, object value, bool allowMultipleValues = false)
        {
            Type valType = GetValueType(type, allowMultipleValues);

            return ConvertValue(valType, value);
        }

        public static T ConvertValue<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }

            var type = typeof(T);

            return (T)ConvertValue(type, value);

        }

        public static object ConvertValue(Type type, object value)
        {
            if (value.GetType() == type || type == null)
            {
                // nothing
            }
            else if (type == typeof(Guid))
            {
                value = new Guid(value.ToString());
            }
            else if (type == typeof(SPFieldMultiChoiceValue))
            {
                value = new SPFieldMultiChoiceValue(value.ToString());
            }
            else if (type == typeof(SPFieldMultiChoiceValue))
            {
                value = new SPFieldMultiChoiceValue(value.ToString());
            }
            else if (type == typeof(SPFieldLookupValue))
            {
                value = new SPFieldLookupValue(value.ToString());
            }
            else if (type == typeof(SPFieldLookupValueCollection))
            {
                value = new SPFieldLookupValueCollection(value.ToString());
            }
            else if (type == typeof(SPFieldUserValue))
            {
                value = new SPFieldUserValue(null, value.ToString());
            }
            else if (type == typeof(SPFieldUserValueCollection))
            {
                value = new SPFieldUserValueCollection(null, value.ToString());
            }
            else if (type == typeof(SPFieldUrlValue))
            {
                value = new SPFieldUrlValue(value.ToString());
            }
            else if (type == typeof(SPContentTypeId))
            {
                value = new SPContentTypeId(value.ToString());
            }
            else if (type == typeof(SPFieldRatingScaleValue))
            {
                value = new SPFieldRatingScaleValue(value.ToString());
            }
            else if (type == typeof(TaxonomyFieldValue))
            {
                value = new TaxonomyFieldValue(value.ToString());
            }
            else if (type == typeof(TaxonomyFieldValueCollection))
            {
                value = new TaxonomyFieldValueCollection(value.ToString());
            }
            else
            {
                value = Convert.ChangeType(value, type);
            }

            return value;
        }
    }
}
