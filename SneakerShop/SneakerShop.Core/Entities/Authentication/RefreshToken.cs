using System;

namespace BaseCamp_WEB_API.Core.Entities.Authentication
{
    /// <summary>
    /// Comprises refresh token data.
    /// </summary>
    public class RefreshToken : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets ID of a user to who this token is attached.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets JWT-token ID.
        /// </summary>
        public string JwtId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this token was used, so we know that it should not generate a new JWT token with the same refresh token.
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it was revoked for security reasons.
        /// </summary>
        public bool IsRevoked { get; set; } // if it has been revoke for security reasons

        /// <summary>
        /// Gets or sets date and time when this token was generated.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets date and time when this token expires.
        /// </summary>
        public DateTime ExpiryDate { get; set; }
    }
}