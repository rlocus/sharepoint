using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Utilities;

namespace SPCore.Helper
{
    /// <summary>
    /// Batch Data Column
    /// </summary>
    public class BatchDataColumn
    {
        private const string ColumnValue = "<SetVar Name=\"urn:schemas-microsoft-com:office:office#{0}\">{1}</SetVar>";

        /// <summary>
        /// Gets or sets the name of the internal.
        /// </summary>
        /// <value>
        /// The name of the internal.
        /// </value>
        public string InternalName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is value HTML.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is value HTML; otherwise, <c>false</c>.
        /// </value>
        public bool IsValueHtml { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchDataColumn" /> class.
        /// </summary>
        /// <param name="internalName">Name of the internal.</param>
        /// <param name="value">The value.</param>
        public BatchDataColumn(string internalName, object value)
        {
            this.InternalName = internalName;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchDataColumn" /> class.
        /// </summary>
        /// <param name="internalName">Name of the internal.</param>
        /// <param name="value">The value.</param>
        /// <param name="isValueHtml">if set to <c>true</c> [is value HTML].</param>
        public BatchDataColumn(string internalName, object value, bool isValueHtml)
        {
            this.InternalName = internalName;
            this.Value = value;
            this.IsValueHtml = isValueHtml;
        }

        public XElement GetColumn()
        {
            var value = this.Value is DateTime
                ? SPUtility.CreateISO8601DateTimeFromSystemDateTime((DateTime)this.Value)
                : this.IsValueHtml ? string.Format("<![CDATA[{0}]]>", this.Value) : this.Value;

            return XElement.Parse(string.Format(ColumnValue, InternalName, value));

            //return new XElement("SetVar",
            //                    new XAttribute("Name", XName.Get(InternalName, "urn:schemas-microsoft-com:office:office")), value);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return GetColumn().ToString(SaveOptions.DisableFormatting);
        }
    }

}
