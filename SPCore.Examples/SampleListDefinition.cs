using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCore.Base;

namespace SPCore.Examples
{
    public class SampleListDefinition : BaseListDefinition
    {
        protected override string DefaultTitle
        {
            get { return "Test List"; }
        }

        protected override string DefaultInternalName
        {
            get
            {
                return "TestList";
            }
        }

        protected override string DefaultDescription
        {
            get { return string.Empty; }
        }

        public SampleListDefinition()
            : base()
        {
            OnQuickLaunch = true;
        }

        protected override void AddFields(SPList list, Action<SPList, IEnumerable<SPField>> fieldsAction)
        {
            //var fields = new SPField[]
            //                 {
            //                     list.ParentWeb.Fields.AddField<SPFieldText>(new Guid("{EE5CC361-1D85-4316-9D23-2F15340990FC}"),  SPFieldType.Text, "Custom Title", "Custom Title",
            //                                                                 false, null)
            //                 };

            //fieldsAction(list, fields);
        }

        protected override void AddContentTypes(SPList list, Action<SPList, IEnumerable<SPContentType>> contentTypesAction)
        {
            SPField[] fields = new SPField[]
                                   {
                                       list.ParentWeb.Fields.AddField
                                           <SPFieldBoolean>(
                                               SPFieldType.Boolean,
                                               "NewBoolean",
                                               "New Boolean",
                                               "Custom Fields", false,
                                               null),
                                       list.ParentWeb.Fields.AddField
                                           <SPFieldDateTime>(
                                               SPFieldType.DateTime,
                                               "NewDate",
                                               "New Date",
                                               "Custom Fields", false,
                                               null),
                                       list.ParentWeb.Fields.AddField
                                           <SPFieldCurrency>(
                                               SPFieldType.Currency,
                                               "NewCurrency",
                                               "New Currency",
                                               "Custom Fields", false,
                                               null),
                                       list.ParentWeb.Fields.AddField
                                           <SPFieldMultiLineText>(
                                               SPFieldType.Note,
                                               "NewNote",
                                               "New Note",
                                               "Custom Fields", false,
                                               null),
                                       list.ParentWeb.Fields.AddField
                                           <SPFieldUser>(
                                               SPFieldType.User,
                                               "NewUser",
                                               "New User",
                                               "Custom Fields", false,
                                               null),
                                       list.ParentWeb.Fields.AddField
                                           <SPFieldNumber>(
                                               SPFieldType.Number,
                                               "NewNumber",
                                               "New Number",
                                               "Custom Fields", false,
                                               null)
                                   };

            SPContentType[] contentTypes = new[]
                                   {
                                       list.ParentWeb.CreateContentType(SPBuiltInContentTypeId.Item,
                                                                        "Custom content type",
                                                                        ct =>
                                                                            {
                                                                                foreach (SPField field in fields)
                                                                                {
                                                                                    ct.AddFieldLink(field.InternalName);
                                                                                }
                                                                            })
                                   };

            contentTypesAction(list, contentTypes);
        }

        protected override void AddViews(SPList list, Action<SPList, IEnumerable<View>> viewsAction)
        {
            //viewsAction(list, new[]
            //                      {
            //                          new View("New View", new[] {"Title", "Custom Title"})
            //                              {
            //                                  IsDefault = true
            //                              }
            //                      });
        }
    }
}
