using System;
using System.Collections.Generic;
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

        private static string ConvertToString(IEnumerable<XElement> elements, SaveOptions saveOption)
        {
            var sb = new StringBuilder();

            foreach (XElement element in elements)
            {
                sb.Append(element.ToString(saveOption));
            }

            return sb.ToString();
        }

        public XElement ToCaml()
        {
            var el = new XElement(QueryTag);

            if (Where != null)
            {
                el.Add(Where.ToXElement());
            }

            if (GroupBy != null)
            {
                el.Add(GroupBy.ToXElement());
            }

            if (OrderBy != null)
            {
                el.Add(OrderBy.ToXElement());
            }

            return el;
        }

        public string ToString(bool includeQueryTag)
        {
            var caml = ToCaml();

            return !includeQueryTag
                ? ConvertToString(caml.Element(QueryTag).Elements(), SaveOption)
                : caml.ToString(SaveOption);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public static implicit operator string(Query query)
        {
            return query.ToString();
        }

        public SPQuery ToSPQuery()
        {
            var query = new SPQuery {Query = this.ToString()};
            return query;
        }
    }
}
