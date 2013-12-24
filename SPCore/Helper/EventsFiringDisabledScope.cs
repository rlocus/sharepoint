using System;
using Microsoft.SharePoint;

namespace SPCore.Helper
{
    /// <summary>
    /// Disables the event firing
    /// </summary>
    public sealed class EventsFiringDisabledScope : SPEventReceiverBase, IDisposable
    {
        readonly bool _previouslyEnabled;

        public EventsFiringDisabledScope()
        {
            _previouslyEnabled = EventFiringEnabled;
            EventFiringEnabled = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            EventFiringEnabled = _previouslyEnabled;
        }

        #endregion
    }
}
