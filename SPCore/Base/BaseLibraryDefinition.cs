using System;
using Microsoft.SharePoint;

namespace SPCore.Base
{
    public abstract class BaseLibraryDefinition : BaseListDefinition
    {
        public bool EnableVersioning { get; set; }
        public bool EnableMinorVersions { get; set; }
        public bool ForceCheckout { get; set; }

        protected BaseLibraryDefinition()
            : base(SPListTemplateType.DocumentLibrary)
        {
        }

        protected BaseLibraryDefinition(SPListTemplate template)
            : base(template)
        {
            if (template.BaseType != SPBaseType.DocumentLibrary)
            {
                throw new NotSupportedException();
            }
        }

        protected BaseLibraryDefinition(SPListTemplateType templateType)
            : base(templateType)
        {
            throw new NotSupportedException();
        }

        public new TList Create<TList>(SPWeb web, string internalName, string title, string listDesc)
            where TList : SPDocumentLibrary
        {
            return base.Create<TList>(web, internalName, title, listDesc);
        }

        public new TList Create<TList>(SPWeb web)
            where TList : SPDocumentLibrary
        {
            return base.Create<TList>(web);
        }

        protected override void SetProperties<TList>(TList list)
        {
            base.SetProperties(list);

            list.EnableVersioning = EnableVersioning;
            list.EnableMinorVersions = EnableMinorVersions;
            list.ForceCheckout = ForceCheckout;
        }
    }
}
