using System;
using Microsoft.SharePoint.Administration;

namespace SPCore.Extensions
{
    public static class ExceptionExtensions
    {
        public static void SPTraceLogError(this Exception ex, string categoryName)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                                                  new SPDiagnosticsCategory(categoryName, TraceSeverity.High, EventSeverity.Error),
                                                  TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
        }

    }
}
