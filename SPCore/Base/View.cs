using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore.Base
{
    public sealed class View
    {
        public string Name { get; private set; }

        public string[] ViewFields { get; private set; }

        public uint RowLimit { get; set; }

        public string Query { get; set; }

        public bool IsDefault { get; set; }

        public SPViewCollection.SPViewType ViewType { get; set; }

        public bool IsPersonal { get; set; }

        public bool Paged { get; set; }

        public bool Hidden { get; set; }

        public SPViewScope Scope { get; set; }

        public View(string name, string[] viewFields)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (viewFields == null) throw new ArgumentNullException("viewFields");

            Name = name;
            ViewFields = viewFields;
            IsDefault = false;
            RowLimit = 30;
            Paged = true;
            ViewType = SPViewCollection.SPViewType.Html;
            IsPersonal = false;
            Hidden = false;
            Scope = SPViewScope.Default;
        }

        public SPView Create(SPList list)
        {
            SPView view = list.GetView(Name);
            IEnumerable<SPField> viewFields = GetViewFields(list);

            if (view == null)
            {
                StringCollection strViewFields = new StringCollection();
                strViewFields.AddRange(viewFields.Select(vf => vf.InternalName).ToArray());
                view = list.Views.Add(Name, strViewFields, Query, RowLimit, Paged, IsDefault, ViewType, IsPersonal);
            }
            else
            {
                view.ViewFields.DeleteAll();

                foreach (SPField viewField in viewFields)
                {
                    view.ViewFields.Add(viewField);
                }
             
                view.Hidden = Hidden;
                view.Scope = Scope;
                view.Query = Query;
                view.RowLimit = RowLimit;
                view.Paged = Paged;
                view.DefaultView = IsDefault;
                view.Update();
            }

            return view;
        }

        private IEnumerable<SPField> GetViewFields(SPList list)
        {
            return ViewFields != null
                       ? ViewFields.Select(viewField => list.Fields.GetField(viewField))
                       : new List<SPField>();
        }
    }
}
