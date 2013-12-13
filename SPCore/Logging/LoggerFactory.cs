using System;

namespace SPCore.Logging
{
    /// <summary>
    /// The factory for Trace loggers.
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFactory"/> class.
        /// </summary>
        /// <param name="isDebugEnabled">if set to <c>true</c> sets if debug is enabled.</param>
        public LoggerFactory(bool isDebugEnabled)
        {
            this.IsDebugEnabled = isDebugEnabled;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is debug enabled.
        /// </summary>
        public bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Create the ILogger for a particular type.
        /// </summary>
        /// <param name="type">The type to create the logger for.</param>
        /// <returns>
        /// The ILogger instance.
        /// </returns>
        public ILogger Create(Type type)
        {
            return new Logger(type.FullName, this.IsDebugEnabled);
        }

        /// <summary>
        /// Create the ILogger for a particular name.
        /// </summary>
        /// <param name="name">The name to create the logger for.</param>
        /// <returns>
        /// The ILogger instance.
        /// </returns>
        public ILogger Create(string name)
        {
            return new Logger(name, this.IsDebugEnabled);
        }
    }
}
