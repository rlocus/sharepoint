using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore.Base
{
    public abstract class BaseListDefinition
    {
        private readonly SPListTemplate _template;
        private readonly SPListTemplateType _templateType;

        protected abstract string DefaultTitle { get; }

        protected virtual string DefaultInternalName { get { return DefaultTitle; } }

        protected abstract string DefaultDescription { get; }

        protected BaseListDefinition()
        {
            Hidden = false;
            OnQuickLaunch = true;
            EnableAttachments = true;
            _templateType = SPListTemplateType.GenericList;
        }

        protected BaseListDefinition(SPListTemplate template)
            : base()
        {
            _template = template;
        }

        protected BaseListDefinition(SPListTemplateType templateType)
            : base()
        {
            _templateType = templateType;
        }

        public bool Hidden { get; set; }
        public bool OnQuickLaunch { get; set; }
        public bool EnableAttachments { get; set; }

        public TList Create<TList>(SPWeb web, string internalName, string title, string listDesc)
            where TList : SPList
        {
            return _template != null
                       ? web.AddList<TList>(internalName, title, listDesc, _template,
                                     lst =>
                                     {
                                         SetProperties(lst);
                                         AddFields(lst, FieldsAction);
                                         AddContentTypes(lst, ContentTypesAction);
                                         AddViews(lst, ViewsAction);
                                     })
                       : web.AddList<TList>(internalName, title, listDesc, _templateType,
                                     lst =>
                                     {
                                         SetProperties(lst);
                                         AddFields(lst, FieldsAction);
                                         AddContentTypes(lst, ContentTypesAction);
                                         AddViews(lst, ViewsAction);
                                     });
        }

        public TList Create<TList>(SPWeb web)
            where TList : SPList
        {
            return Create<TList>(web, DefaultInternalName, DefaultTitle, DefaultDescription);
        }

        protected virtual void SetProperties<TList>(TList list)
             where TList : SPList
        {
            list.OnQuickLaunch = OnQuickLaunch;
            list.Hidden = Hidden;
            list.EnableAttachments = EnableAttachments;
        }

        protected virtual void AddFields(SPList list, Action<SPList, IEnumerable<SPField>> fieldsAction)
        {
            if (fieldsAction != null) fieldsAction(list, new SPField[0]);
        }

        protected virtual void AddContentTypes(SPList list, Action<SPList, IEnumerable<SPContentType>> contentTypesAction)
        {
            if (contentTypesAction != null) contentTypesAction(list, new SPContentType[0]);
        }

        protected virtual void AddViews(SPList list, Action<SPList, IEnumerable<View>> viewsAction)
        {
            if (viewsAction != null) viewsAction(list, new View[0]);
        }

        private void FieldsAction(SPList list, IEnumerable<SPField> fields)
        {
            foreach (SPField field in fields.Where(field => !list.Fields.Contains(field.Id)))
            {
                list.Fields.Add(field);
            }
        }

        private void ContentTypesAction(SPList list, IEnumerable<SPContentType> contentTypes)
        {
            foreach (SPContentType contentType in contentTypes)
            {
                if (!list.ContainsContentTypeWithId(contentType.Id) && list.IsContentTypeAllowed(contentType))
                {
                    list.AddContentType(contentType);
                }
            }
        }

        private void ViewsAction(SPList list, IEnumerable<View> views)
        {
            foreach (View view in views)
            {
                view.Create(list);
            }
        }
    }
}
