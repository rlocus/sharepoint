using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Helper;

namespace SPCore.Extensions
{
    class SPFieldExtensions
    {
    }

    public static class SPFieldLookupExtensions
    {
        public static SPWeb GetLookupWeb(this SPFieldLookup lookupField)
        {
            SPWeb lookupWeb = null;
            if (lookupField != null)
            {
                SPWeb web = lookupField.ParentList.ParentWeb;
                lookupWeb = lookupField.LookupWebId == web.ID
                                 ? web
                                 : web.Site.OpenWeb(lookupField.LookupWebId);
            }
            return lookupWeb;
        }

        public static SPList GetLookupList(this SPFieldLookup lookupField, SPWeb lookupWeb)
        {
            SPList lookupList = null;

            if (lookupField != null)
            {
                SPList currentList = lookupField.ParentList;

                if (currentList != null)
                {
                    SPWeb currentWeb = lookupField.ParentList.ParentWeb;

                    if (lookupWeb == null)
                    {
                        lookupWeb = currentWeb;
                    }

                    if (SPHelper.IsGuid(lookupField.LookupList))
                    {
                        Guid lookupListId = new Guid(lookupField.LookupList);
                        lookupList = (lookupWeb.ID == currentWeb.ID && lookupListId == currentList.ID)
                                         ? currentList
                                         : lookupWeb.Lists[lookupListId];
                    }
                    else
                    {
                        string lookupListTitle = lookupField.LookupList;
                        lookupList = (lookupWeb.ID == currentWeb.ID && lookupListTitle == currentList.Title)
                                         ? currentList
                                         : lookupWeb.Lists[lookupListTitle];

                    }
                }
            }
            return lookupList;
        }

        public static void DoActionWithLookupList(this SPFieldLookup lookupField, Action<SPList> action)
        {
            if (lookupField == null || action == null) { return; }

            SPWeb lookupWeb = lookupField.GetLookupWeb();

            if (lookupWeb == null) { return; }

            try
            {
                SPList lookupList = lookupField.GetLookupList(lookupWeb);
                action(lookupList);
            }
            finally
            {
                if (lookupWeb != SPContext.Current.Web)
                {
                    lookupWeb.Dispose();
                }
            }
        }

        public static void UpdateLookupReferences(this SPFieldLookup lookupField, SPList lookupList, string lookupName)
        {
            if (string.IsNullOrEmpty(lookupField.LookupList))
            {
                lookupField.LookupWebId = lookupList.ParentWeb.ID;
                lookupField.LookupList = lookupList.ID.ToString();

                if (!string.IsNullOrEmpty(lookupName))
                {
                    lookupField.LookupField = lookupName;
                }
            }
            else
            {
                XElement fieldSchema = XElement.Parse(lookupField.SchemaXml);

                ChangeAttribute(fieldSchema, "WebId", lookupList.ParentWeb.ID);
                ChangeAttribute(fieldSchema, "List", lookupList.ID);

                if (!string.IsNullOrEmpty(lookupName))
                {
                    ChangeAttribute(fieldSchema, "ShowField", lookupName);
                }

                lookupField.SchemaXml = fieldSchema.ToString(SaveOptions.DisableFormatting);
            }

            lookupField.Update(true);
        }

        private static void ChangeAttribute(XElement el, string attrName, object value)
        {
            XAttribute attr = el.Attribute(attrName);

            if (attr == null)
            {
                el.Add(new XAttribute(attrName, value));
            }
            else
            {
                attr.SetValue(value);
            }
        }
    }
}
