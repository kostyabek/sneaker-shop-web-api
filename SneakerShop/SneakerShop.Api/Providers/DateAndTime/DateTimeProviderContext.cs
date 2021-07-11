using System;
using System.Collections;
using System.Threading;

namespace BaseCamp_Web_API.Api.Providers.DateAndTime
{
    /// <summary>
    /// Contains functionality for setting custom date and time in correspondent contexts of use.
    /// </summary>
    public class DateTimeProviderContext : IDisposable
    {
        /// <summary>
        /// Contains date and time set for the current context.
        /// </summary>
        public DateTime ContextDateTimeUtcNow;

        private static ThreadLocal<Stack> _threadScopeStack = new (() => new Stack());

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeProviderContext"/> class.
        /// </summary>
        /// <param name="contextDateTimeUtcNow">Date and time to be set for current context.</param>
        public DateTimeProviderContext(DateTime contextDateTimeUtcNow)
        {
            ContextDateTimeUtcNow = contextDateTimeUtcNow;
            _threadScopeStack.Value.Push(this);
        }

        /// <summary>
        /// Gets the most recent context instance.
        /// </summary>
        public static DateTimeProviderContext Current
        {
            get
            {
                if (_threadScopeStack.Value.Count == 0)
                {
                    return null;
                }

                return (DateTimeProviderContext)_threadScopeStack.Value.Peek();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _threadScopeStack.Value.Pop();
        }
    }
}