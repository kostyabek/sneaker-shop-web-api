namespace BaseCamp_WEB_API.Core.Filters
{
    /// <summary>
    /// Contains offset and limit for database queries.
    /// </summary>
    public class PaginationFilter
    {
        /// <summary>
        /// Gets or sets offset of a query.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets limit of a query.
        /// </summary>
        public int Limit { get; set; }
    }
}