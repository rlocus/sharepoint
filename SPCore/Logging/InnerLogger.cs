using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace SPCore.Logging
{
    public enum CategoryId
    {
        None = 0,
        Debugging,
        Information,
        Warning,
        Error,
        Faulting,
        Fatal
    }

    internal class InnerLogger : SPDiagnosticsServiceBase//SPDiagnosticsService
    {
        public InnerLogger(string name)
            : base(name, SPFarm.Local)
        {
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            //foreach (SPDiagnosticsArea area in base.ProvideAreas())
            //{
            //    yield return area;
            //}

            List<SPDiagnosticsCategory> categories = new List<SPDiagnosticsCategory>();
            categories.Add(new SPDiagnosticsCategory(CategoryId.None.ToString(), TraceSeverity.Verbose, EventSeverity.None, 0, (uint)Enum.Parse(typeof(CategoryId), CategoryId.None.ToString())));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Debugging.ToString(), TraceSeverity.Medium, EventSeverity.Information, 0, (uint)Enum.Parse(typeof(CategoryId), CategoryId.Debugging.ToString())));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Error.ToString(), TraceSeverity.Medium, EventSeverity.Information, 0, (uint)Enum.Parse(typeof(CategoryId), CategoryId.Error.ToString())));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Fatal.ToString(), TraceSeverity.High, EventSeverity.Information, 0, (uint)Enum.Parse(typeof(CategoryId), CategoryId.Fatal.ToString())));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Faulting.ToString(), TraceSeverity.Medium, EventSeverity.Verbose, 0, (uint)Enum.Parse(typeof(CategoryId), CategoryId.Faulting.ToString())));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Information.ToString(), TraceSeverity.Verbose, EventSeverity.Information, 0, (uint)Enum.Parse(typeof(CategoryId), CategoryId.Information.ToString())));
            categories.Add(new SPDiagnosticsCategory(CategoryId.Warning.ToString(), TraceSeverity.Medium, EventSeverity.Information, 0, (uint)Enum.Parse(typeof(CategoryId), CategoryId.Warning.ToString())));

            yield return new SPDiagnosticsArea(Name, categories);
        }

    }
}
