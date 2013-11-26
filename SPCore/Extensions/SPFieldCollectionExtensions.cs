using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class SPFieldCollectionExtensions
    {
        public static string AddField(
          this SPFieldCollection fields,
          SPFieldType fieldType,
          string name,
          bool required,
          bool overwrite)
        {
            if (overwrite && fields.ContainsField(name))
            {
                fields.Delete(name);
            }

            name = fields.Add(name, fieldType, required);
            return name;
        }

        public static TF AddField<TF>(
           this SPFieldCollection fields,
           SPFieldType fieldType,
           string name,
           bool required,
           Action<TF> action)
           where TF : SPField
        {
            //name = SPHelper.ConvertHebrewToUnicodeHex(name);

            if (!fields.ContainsField(name))
            {
                name = AddField(fields, fieldType, name, required, false);
            }

            //TF field = (TF)fields.GetFieldByInternalName(name);
            TF field = (TF)fields.GetField(name);

            if (action != null)
            {
                action(field);
            }

            field.Update();
            return field;
        }

        public static TF AddCustomField<TF>(
           this SPFieldCollection fields,
           string typeName,
           string name,
           bool required,
           Action<TF> action)
           where TF : SPField
        {
            //name = SPHelper.ConvertHebrewToUnicodeHex(name);

            if (!fields.ContainsField(name))
            {
                name = fields.Add(fields.CreateNewField(typeName, name));
            }

            //TF field = (TF)fields.GetFieldByInternalName(name);
            TF field = (TF)fields.GetField(name);

            field.Required = required;

            if (action != null)
            {
                action(field);
            }

            field.Update();
            return field;
        }

        public static TF AddField<TF>(
            this SPFieldCollection fields,
            SPFieldType fieldType,
            string internalName,
            string displayName,
            string groupName,
            bool required,
            Action<TF> action)
            where TF : SPField
        {
            return fields.AddField<TF>(fieldType, internalName, required,
                                       field =>
                                       {
                                           field.Title = displayName;

                                           if (!string.IsNullOrEmpty(groupName))
                                           {
                                               field.Group = groupName;
                                           }

                                           if (action != null)
                                           {
                                               action(field);
                                           }
                                       });
        }

        public static TF AddField<TF>(
          this SPFieldCollection fields,
          Guid fieldId,
          SPFieldType fieldType,
          string displayName,
          string name,
          bool required,
          Action<TF> action)
          where TF : SPField
        {
            //name = SPHelper.ConvertHebrewToUnicodeHex(name);

            if (!fields.Contains(fieldId))
            {
                string fieldXml =
                    string.Format(
                        @"<Field ID=""{0}"" Name=""{1}"" StaticName=""{1}"" DisplayName=""{2}"" Type=""{3}"" Overwrite=""TRUE"" SourceID=""http://schemas.microsoft.com/sharepoint/v3"" />",
                    fieldId, name, displayName, fieldType);
                name = fields.AddFieldAsXml(fieldXml);
            }

            //TF field = (TF)fields.GetFieldByInternalName(name);
            TF field = (TF)fields.GetField(name);
            field.Required = required;

            if (action != null)
            {
                action(field);
            }

            field.Update();
            return field;
        }

        public static TSPField CreateLookup<TSPField>(this SPFieldCollection fields, Guid lookupListId, string name, bool required, Action<TSPField> action) where TSPField : SPFieldLookup
        {
            TSPField field = !fields.ContainsField(name)
                                 ? (TSPField)fields.GetField(fields.AddLookup(name, lookupListId, required))
                                 : (TSPField)fields.GetField(name);

            if (action != null)
            {
                action(field);
            }

            field.Update();
            return field;
        }

        public static TSPField CreateLookup<TSPField>(this SPFieldCollection fields,
                Guid lookupListId, string internalName, string displayName, bool required,
                Action<TSPField> action) where TSPField : SPFieldLookup
        {
            TSPField field = CreateLookup<TSPField>(fields, lookupListId, internalName, required,
                                                    (f) =>
                                                    {
                                                        f.Title = displayName;
                                                        f.Required = required;
                                                    });
            return field;
        }

        public static List<string> GetChoiceFieldValues(this SPFieldCollection fields, Guid fieldId)
        {
            SPFieldChoice field = (SPFieldChoice)fields[fieldId];
            return field.Choices.Cast<string>().ToList();
        }

        public static List<string> GetChoiceFieldValues(this SPFieldCollection fields, string fieldName)
        {
            SPFieldChoice field = (SPFieldChoice)fields.GetField(fieldName);
            return field.Choices.Cast<string>().ToList();
        }

        public static SPField TryGetField(this SPFieldCollection fields, Guid fieldId)
        {
            return TryGetField<SPField>(fields, fieldId);
        }

        public static TF TryGetField<TF>(this SPFieldCollection fields, Guid fieldId)
            where TF : SPField
        {
            try
            {
                return (TF)fields[fieldId];
            }
            catch
            {
                return null;
            }
        }

        public static SPField TryGetField(this SPFieldCollection fields, string fieldName)
        {
            return TryGetField<SPField>(fields, fieldName);
        }

        public static TF TryGetField<TF>(this SPFieldCollection fields, string fieldName)
           where TF : SPField
        {
            try
            {
                return (TF)fields.GetField(fieldName);
            }
            catch
            {
                return null;
            }
        }
    }
}
