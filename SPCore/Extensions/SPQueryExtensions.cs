using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class SPQueryExtensions
    {
        public static readonly string EmptyCamlQuery = "0";

        public static SPQuery GetView(this SPQuery spQuery, SPViewScope viewScope, bool hideUnapproved = false)
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

        public static SPQuery AddWhereCondition(this SPQuery spQuery, string conditionQuery)
        {
            XElement query = null;
            bool withoutQuery = false;

            if (!string.IsNullOrEmpty(spQuery.Query))
            {
                query = XElement.Parse(string.Format("<Query>{0}</Query>", spQuery.Query));
                withoutQuery = true;
            }

            XElement condition = XElement.Parse(conditionQuery);

            if (query != null && !query.HasElements)
            {
                spQuery.Query = new XElement("Where", condition).ToString();
            }
            else
            {
                if (query != null)
                {
                    XElement where = query.DescendantsAndSelf("Where").FirstOrDefault();

                    if (where != null)
                    {
                        XElement existingCondition = where.Elements().FirstOrDefault();
                        condition = existingCondition == null
                                        ? condition
                                        : new XElement("And", condition, existingCondition);
                        where.ReplaceAll(condition);
                    }
                    else
                    {
                        query.Add(new XElement("Where", condition));
                    }
                }
                else
                {
                    query = new XElement("Where", condition);
                }

                if (withoutQuery)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (XElement element in query.DescendantsAndSelf("Query").Elements())
                    {
                        sb.Append(element.ToString(SaveOptions.DisableFormatting));
                    }

                    spQuery.Query = sb.ToString();
                }
                else
                {
                    spQuery.Query = query.ToString();
                }
            }
            return spQuery;
        }
    }
}
