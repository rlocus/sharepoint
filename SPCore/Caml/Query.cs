using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Caml.Clauses;

namespace SPCore.Caml
{
    // var query = new Query()
    //{
    //    Where = new Where(
    //        new Or(
    //            new Eq<string>(fieldName: "ContentType", value: "My Content Type", type: SPFieldType.Text),
    //            new IsNotNull(fieldName: "Description"))),

    //    GroupBy = new GroupBy(fieldName: "Title", collapsed: true),
    //    OrderBy = new OrderBy(fieldName: "_Author").ThenBy(fieldName: "AuthoringDate").ThenBy(fieldName: "AssignedTo")
    //};

    public sealed class Query
    {
        public Where Where { get; set; }
        public OrderBy OrderBy { get; set; }
        public GroupBy GroupBy { get; set; }

        private const string QueryTag = "Query";

        public bool DisableFormatting;

        private SaveOptions SaveOption
        {
            get
            {
                return DisableFormatting ? SaveOptions.DisableFormatting : SaveOptions.None;
            }
        }

        private string ConvertToString(IEnumerable<XElement> elements, SaveOptions saveOption)
        {
            var sb = new StringBuilder();

            foreach (XElement element in elements)
            {
                if (DisableFormatting)
                {
                    sb.Append(element.ToString(saveOption));
                }
                else
                {
                    sb.AppendLine(element.ToString(saveOption));
                }
            }

            return sb.ToString();
        }

        private XElement ToCaml()
        {
            var el = new XElement(QueryTag);

            if (Where != null)
            {
                el.Add(Where.ToXElement());
            }

            if (OrderBy != null)
            {
                el.Add(OrderBy.ToXElement());
            }

            if (GroupBy != null)
            {
                el.Add(GroupBy.ToXElement());
            }

            return el;
        }

        public string ToString(bool includeQueryTag)
        {
            XElement caml = ToCaml();
            IEnumerable<XElement> elements = caml.Elements();
            return !includeQueryTag
                ? ConvertToString(elements, SaveOption)
                : caml.ToString(SaveOption);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public static implicit operator string(Query query)
        {
            return query != null ? query.ToString() : string.Empty;
        }

        public SPQuery ToSPQuery()
        {
            var query = new SPQuery { Query = this.ToString() };
            return query;
        }

        public SPQuery ToSPQuery(params string[] viewFields)
        {
            SPQuery query = ToSPQuery();
            return query.WithViewFields(viewFields);
        }

        public static Query Parse(string existingQuery)
        {
            if (string.IsNullOrEmpty(existingQuery))
            {
                return null;
            }

            XElement el = XElement.Parse(existingQuery, LoadOptions.None);
            return Parse(el);
        }

        public static Query Parse(XElement existingQuery)
        {
            Query query = new Query();

            if (existingQuery != null && (existingQuery.HasElements && existingQuery.Name.LocalName == QueryTag))
            {
                XElement existingWhere = existingQuery.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "Where", StringComparison.InvariantCultureIgnoreCase));

                if (existingWhere != null) query.Where = new Where(existingWhere);

                XElement existingOrderBy = existingQuery.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "OrderBy", StringComparison.InvariantCultureIgnoreCase));

                if (existingOrderBy != null) query.OrderBy = new OrderBy(existingOrderBy);

                XElement existingGroupBy = existingQuery.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "GroupBy", StringComparison.InvariantCultureIgnoreCase));

                if (existingGroupBy != null) query.GroupBy = new GroupBy(existingGroupBy);
            }

            return query;
        }

        public static Query Combine(Query firstQuery, Query secondQuery)
        {
            return new Query
                       {
                           Where = Where.Combine(firstQuery.Where, secondQuery.Where),
                           OrderBy = OrderBy.Combine(firstQuery.OrderBy, secondQuery.OrderBy),
                           GroupBy = GroupBy.Combine(firstQuery.GroupBy, secondQuery.GroupBy)
                       };
        }
    }
}
