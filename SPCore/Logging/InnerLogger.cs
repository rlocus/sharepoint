using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;

namespace SPCore.Logging
{
    public enum CategoryId : uint
    {
        None = 0,
        Fatal = 1,
        Error = 2,
        Warn = 3,
        Info = 4,
        Debug = 5
    }

    [Guid("111C3DB0-8C36-46B3-8B87-2EB7719114ED")]
    internal class InnerLogger : SPDiagnosticsServiceBase
    {
        private static InnerLogger _current;

        public InnerLogger()
            : this("SP Logging Service")
        {
        }

        public InnerLogger(string name)
            : this(name, SPFarm.Local)
        {
        }

        public InnerLogger(string name, SPFarm farm)
            : base(name, farm)
        {
        }

        public static InnerLogger Local
        {
            get { return _current ?? (_current = GetLocal<InnerLogger>()); }
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsCategory> categories = new List<SPDiagnosticsCategory>();
            categories.Add(new SPDiagnosticsCategory(CategoryId.None.ToString(), TraceSeverity.Verbose, EventSeverity.None, 0, (uint)CategoryId.None));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Fatal.ToString(), TraceSeverity.High, EventSeverity.Information, 0, (uint)CategoryId.Fatal));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Error.ToString(), TraceSeverity.Medium, EventSeverity.Information, 0, (uint)CategoryId.Error));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Warn.ToString(), TraceSeverity.Medium, EventSeverity.Information, 0, (uint)CategoryId.Warn));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Info.ToString(), TraceSeverity.Verbose, EventSeverity.Information, 0, (uint)CategoryId.Info));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Debug.ToString(), TraceSeverity.VerboseEx, EventSeverity.Information, 0, (uint)CategoryId.Debug));
            yield return new SPDiagnosticsArea(Name, categories);
        }

    }
}
