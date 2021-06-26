namespace BaseCamp_WEB_API.Core.Entities
{
    /// <summary>
    /// Represents vendor data.
    /// </summary>
    public class Vendor : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets vendor name.
        /// </summary>
        public string Name { get; set; }
    }
}