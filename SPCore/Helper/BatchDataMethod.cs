using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace SPCore.Helper
{
    /// <summary>
    /// Defines Batch Data Method.
    /// </summary>
    public class BatchDataMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchDataMethod" /> class.
        /// </summary>
        public BatchDataMethod()
        {
            this.ColumnsData = new List<BatchDataColumn>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the list id.
        /// </summary>
        /// <value>
        /// The list id.
        /// </value>
        public Guid ListId { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public BatchDataCommandType Command { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public int ItemId { get; set; }

        public string FileRef { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public List<BatchDataColumn> ColumnsData { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return GetMethod().ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Gets the method XML.
        /// </summary>
        /// <returns></returns>
        public XElement GetMethod()
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                this.Id = Guid.NewGuid().ToString();
            }

            List<XElement> columns = new List<XElement>();

            this.ColumnsData.ForEach(c => columns.Add(c.GetColumn()));

            string methodItemId = this.Command == BatchDataCommandType.Add ? "New" : this.ItemId.ToString(CultureInfo.InvariantCulture);

            XElement method;
            if (this.Command == BatchDataCommandType.Delete)
            {
                method = new XElement("Method", new XAttribute("ID", this.Id),
                                      new XElement("SetList", new XAttribute("Scope", "Request"), this.ListId),
                                      new XElement("SetVar", new XAttribute("Name", "Cmd"),
                                                   GetCommandForMethod(this.Command)),
                                      new XElement("SetVar", new XAttribute("Name", "ID"), methodItemId)
                                      );
                if (!string.IsNullOrEmpty(this.FileRef))
                {
                    method.Add(new XElement("SetVar", new XAttribute("Name", "owsfileref"), this.FileRef));
                }
            }
            else
            {
                method = new XElement("Method", new XAttribute("ID", this.Id),
                                      new XElement("SetList", this.ListId),
                                      new XElement("SetVar", new XAttribute("Name", "Cmd"),
                                                   GetCommandForMethod(this.Command)),
                                      new XElement("SetVar", new XAttribute("Name", "ID"), methodItemId),
                                      columns);
            }
            return method;
        }

        /// <summary>
        /// Gets the command for method.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid command!</exception>
        private static string GetCommandForMethod(BatchDataCommandType command)
        {
            switch (command)
            {
                case BatchDataCommandType.Add:
                case BatchDataCommandType.Update:
                    return "Save";
                case BatchDataCommandType.Delete:
                    return "Delete";
            }

            throw new ArgumentException("Invalid command!");
        }
    }
}
