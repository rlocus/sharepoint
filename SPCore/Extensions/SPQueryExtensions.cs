using System.Text;
using Microsoft.SharePoint;
using SPCore.Caml;

namespace SPCore
{
    public static class SPQueryExtensions
    {
        public static readonly string EmptyCamlQuery = "0";

        public static SPQuery WithViewScope(this SPQuery spQuery, SPViewScope viewScope, bool hideUnapproved = false)
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

        public static SPQuery WithViewFields(this SPQuery spQuery, params string[] viewFields)
        {
            if (viewFields.Length > 0)
            {
                spQuery.ViewFieldsOnly = true;

                StringBuilder sb = new StringBuilder();

                foreach (string viewField in viewFields)
                {
                    sb.Append(new FieldRef() { Name = viewField }.ToString());
                }

                spQuery.ViewFields = sb.ToString();
            }

            return spQuery;
        }

        public static SPQuery WithQuery(this SPQuery spQuery, Query query, bool addToExisting = false)
        {
            if (spQuery != null)
            {
                if (addToExisting)
                {
                    Query existingQuery = spQuery.GetQueryObject();
                    spQuery.Query = Query.Combine(existingQuery, query).ToString(false);
                }
                else
                {
                    spQuery.Query = query.ToString(false);
                }
            }

            return spQuery;
        }

        public static Query GetQueryObject(this SPQuery spQuery)
        {
            if (spQuery != null)
            {
                return Query.Parse(string.Format("<Query>{0}</Query>", spQuery.Query));
            }

            return null;
        }

        //public static SPQuery AddWhereCondition(this SPQuery spQuery, string conditionQuery)
        //{
        //    XElement query = null;
        //    bool withoutQuery = false;

        //    if (!string.IsNullOrEmpty(spQuery.Query))
        //    {
        //        query = XElement.Parse(string.Format("<Query>{0}</Query>", spQuery.Query));
        //        withoutQuery = true;
        //    }

        //    XElement condition = XElement.Parse(conditionQuery);

        //    if (query != null && !query.HasElements)
        //    {
        //        spQuery.Query = new XElement("Where", condition).ToString();
        //    }
        //    else
        //    {
        //        if (query != null)
        //        {
        //            XElement where = query.DescendantsAndSelf("Where").FirstOrDefault();

        //            if (where != null)
        //            {
        //                XElement existingCondition = where.Elements().FirstOrDefault();
        //                condition = existingCondition == null
        //                                ? condition
        //                                : new XElement("And", condition, existingCondition);
        //                where.ReplaceAll(condition);
        //            }
        //            else
        //            {
        //                query.Add(new XElement("Where", condition));
        //            }
        //        }
        //        else
        //        {
        //            query = new XElement("Where", condition);
        //        }

        //        if (withoutQuery)
        //        {
        //            StringBuilder sb = new StringBuilder();

        //            foreach (XElement element in query.DescendantsAndSelf("Query").Elements())
        //            {
        //                sb.Append(element.ToString(SaveOptions.DisableFormatting));
        //            }

        //            spQuery.Query = sb.ToString();
        //        }
        //        else
        //        {
        //            spQuery.Query = query.ToString();
        //        }
        //    }
        //    return spQuery;
        //}
    }
}
