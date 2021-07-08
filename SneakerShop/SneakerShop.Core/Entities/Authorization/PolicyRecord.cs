namespace BaseCamp_WEB_API.Core.Entities.Authorization
{
    /// <summary>
    /// Represents a database record of a policy.
    /// </summary>
    public class PolicyRecord : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets policy name.
        /// </summary>
        public string Name { get; set; }
    }
}