using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPCore.Logging
{
    /// <summary>
    /// A logger that logs to SharePoint's ULS.
    /// </summary>
    public class Logger : ILogger
    {
        private InnerLogger _innerLogger;
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isDebugEnabled">if set to <c>true</c> [is debug enabled].</param>
        public Logger(string name, bool isDebugEnabled = false)
        {
            this.Name = name;
            this.IsDebugEnabled = isDebugEnabled;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns TRUE if debug-level logging is enabled.
        /// </summary>
        public bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Output the message at the Debug level.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Debug(object message)
        {
            if (this.IsDebugEnabled)
            {
                this.Debug("{0}", message);
            }
        }

        /// <summary>
        /// Output the formatted message at the Debug level.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="args">The arguments to pass to the formatter.</param>
        public void Debug(string format, params object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.InnerLog(CategoryId.Debugging, TraceSeverity.Verbose, format, args);
            }
        }

        /// <summary>
        /// Output the message at the Info level.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Info(object message)
        {
            this.Info("{0}", message);
        }

        /// <summary>
        /// Output the formatted message at the Info level.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="args">The arguments to pass to the formatter.</param>
        public void Info(string format, params object[] args)
        {
            this.InnerLog(CategoryId.Information, TraceSeverity.Medium, format, args);
        }

        /// <summary>
        /// Output the message at the Warn level.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Warn(object message)
        {
            this.Warn("{0}", message);
        }

        /// <summary>
        /// Output the formatted message at the Warn level.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="args">The arguments to pass to the formatter.</param>
        public void Warn(string format, params object[] args)
        {
            this.InnerLog(CategoryId.Warning, TraceSeverity.High, format, args);
        }

        /// <summary>
        /// Output the message at the Error level.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Error(object message)
        {
            this.Error("{0}", message);
        }

        /// <summary>
        /// Output the formatted message at the Error level.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="args">The arguments to pass to the formatter.</param>
        public void Error(string format, params object[] args)
        {
            this.InnerLog(CategoryId.Error, TraceSeverity.Unexpected, format, args);
        }

        /// <summary>
        /// Output the message at the Fatal level.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Fatal(object message)
        {
            this.Fatal("{0}", message);
        }

        /// <summary>
        /// Output the formatted message at the Fatal level.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="args">The arguments to pass to the formatter.</param>
        public void Fatal(string format, params object[] args)
        {
            this.InnerLog(CategoryId.Fatal, TraceSeverity.Unexpected, format, args);
        }

        /// <summary>
        /// Logs to the ULS.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="traceSeverity">The trace severity.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The message arguments.</param>
        protected virtual void InnerLog(CategoryId categoryId, TraceSeverity traceSeverity, string message, params object[] args)
        {
            SPSecurity.RunWithElevatedPrivileges(
                () =>
                {
                    if (_innerLogger == null)
                    {
                        _innerLogger = new InnerLogger(Name);
                    }

                    SPDiagnosticsCategory category =
                        _innerLogger.Areas[_innerLogger.Name].Categories[categoryId.ToString()];
                    _innerLogger.WriteTrace(0, category, traceSeverity, message, args);
                });
        }
    }
}
