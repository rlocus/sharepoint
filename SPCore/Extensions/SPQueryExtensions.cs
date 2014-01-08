using System.Text;
using Microsoft.SharePoint;
using SPCore.Caml;

namespace SPCore
{
    public static class SPQueryExtensions
    {
        public static readonly string EmptyCamlQuery = "0";

        public static SPQuery InScope(this SPQuery spQuery, SPViewScope viewScope, bool hideUnapproved = false)
        {
            switch (viewScope)
            {
                case SPViewScope.Default:
                    spQuery.ViewAttributes = hideUnapproved
                                                 ? "ModerationType=\"HideUnapproved\""
                                                 : null;
                    break;
                case SPViewScope.FilesOnly:
                case SPViewScope.Recursive:
                case SPViewScope.RecursiveAll:
                    spQuery.ViewAttributes = hideUnapproved
                                                 ? string.Format("Scope=\"{0}\" ModerationType=\"HideUnapproved\"",
                                                                 viewScope)
                                                 : string.Format("Scope=\"{0}\"", viewScope);
                    break;
            }

            return spQuery;
        }

        public static SPQuery Include(this SPQuery spQuery, params string[] viewFields)
        {
            if (viewFields.Length > 0)
            {
                spQuery.ViewFieldsOnly = true;

                StringBuilder sb = new StringBuilder();

                foreach (string viewField in viewFields)
                {
                    sb.Append(new FieldRef() { Name = viewField });
                }

                spQuery.ViewFields = sb.ToString();
            }

            return spQuery;
        }

        public static SPQuery Combine(this SPQuery spQuery, Query query)
        {
            if (spQuery != null)
            {
                Query existingQuery = Query.GetFromSPQuery(spQuery);
                spQuery.Query = Query.Combine(existingQuery, query).ToString(false);
            }

            return spQuery;
        }
    }
}
