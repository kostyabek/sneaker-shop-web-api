namespace BaseCamp_WEB_API.Core.Entities
{
    /// <summary>
    /// Represents data about a season.
    /// </summary>
    public class Season : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets season name.
        /// </summary>
        public string Name { get; set; }
    }
}