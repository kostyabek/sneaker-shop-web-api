using SqlKata;

namespace BaseCamp_WEB_API.Core.Entities
{
    /// <summary>
    /// Represents the ID of every entity related with the database.
    /// </summary>
    /// <typeparam name="T">Data type of the entity's ID.</typeparam>
    public abstract class BaseModel<T>
    {
        /// <summary>
        /// Gets or sets an ID of the entity.
        /// </summary>
        /*[Ignore]*/
        public T Id { get; set; }
    }
}