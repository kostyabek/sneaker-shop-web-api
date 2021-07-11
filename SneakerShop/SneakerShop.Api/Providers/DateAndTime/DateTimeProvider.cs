using System;

namespace BaseCamp_Web_API.Api.Providers.DateAndTime
{
    /// <summary>
    /// Provides date and time depending on the context of use.
    /// </summary>
    public class DateTimeProvider
    {
        private static Lazy<DateTimeProvider> _lazyInstance = new (() => new DateTimeProvider());
        private readonly Func<System.DateTime> _defaultCurrentFunction = () => System.DateTime.UtcNow;

        private DateTimeProvider()
        {
        }

        /// <summary>
        /// Gets return the instance of the provider.
        /// </summary>
        public static DateTimeProvider Instance => _lazyInstance.Value;

        /// <summary>
        /// Gives current date and time.
        /// </summary>
        /// <returns></returns>
        public System.DateTime GetUtcNow()
        {
            if (DateTimeProviderContext.Current == null)
            {
                return _defaultCurrentFunction.Invoke();
            }

            return DateTimeProviderContext.Current.ContextDateTimeUtcNow;
        }
    }
}