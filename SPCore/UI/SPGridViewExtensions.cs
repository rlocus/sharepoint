using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SPCore.UI
{
    public static class SPGridViewExtensions
    {
        public static void GenerateColumns(this SPGridView gridView, SPList list)
        {
            GenerateColumns(gridView, list.DefaultView);
        }

        public static void GenerateColumns(this SPGridView gridView, SPView view)
        {
            GenerateColumns(gridView, view.ParentList, view.ViewFields.Cast<string>());
        }

        public static void GenerateColumns(this SPGridView gridView, SPList list, IEnumerable<string> fieldNames)
        {
            gridView.AutoGenerateColumns = false;

            foreach (string fieldName in fieldNames)
            {
                SPField field = list.Fields.GetFieldByInternalName(fieldName);
                BoundField column = new BoundField
                {
                    DataField = field.InternalName,
                    HeaderText = field.Title
                };
                gridView.Columns.Add(column);
            }
        }
    }
}
