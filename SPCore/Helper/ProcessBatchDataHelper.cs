using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Helper
{
    /// <summary>
    /// <see cref="SPWeb"/> ProcessBatchData Helper class.
    /// </summary>
    public static class ProcessBatchDataHelper
    {
        /// <summary>
        /// The batch format
        /// </summary>
        private const string BatchFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ows:Batch OnError=\"{0}\">{1}</ows:Batch>";

        /// <summary>
        /// Gets the batch with return on error by default.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        public static string GetBatch(List<BatchDataMethod> methods)
        {
            return GetBatch(methods, OnErrorAction.Return);
        }

        /// <summary>
        /// Gets the batch.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="errorAction">The error action.</param>
        /// <returns></returns>
        public static string GetBatch(List<BatchDataMethod> methods, OnErrorAction errorAction)
        {
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));

            XElement batch = new XElement(XName.Get("Batch"));
            batch.Add(new XAttribute("OnError", OnErrorString(errorAction)));
            methods.ForEach(m => batch.Add(m.GetMethod()));

            xDoc.Add(batch);

            //return xDoc.ToString(SaveOptions.DisableFormatting);

            StringBuilder batchStr = new StringBuilder();
            methods.ForEach(m => batchStr.Append(m.ToString()));

            return string.Format(BatchFormat, OnErrorString(errorAction), batchStr);
        }

        /// <summary>
        /// Called when [error string].
        /// </summary>
        /// <param name="errorAction">The error action.</param>
        /// <returns>The on error action.</returns>
        private static string OnErrorString(OnErrorAction errorAction)
        {
            switch (errorAction)
            {
                case OnErrorAction.Continue:
                    return "Continue";
                case OnErrorAction.Return:
                default:
                    return "Return";
            }
        }
    }
}
