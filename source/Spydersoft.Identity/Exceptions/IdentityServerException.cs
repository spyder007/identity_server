using System;

namespace Spydersoft.Identity.Exceptions
{
    /// <summary>
    /// Class IdentityServerException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public class IdentityServerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServerException"/> class.
        /// </summary>
        public IdentityServerException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public IdentityServerException(string message) : base(message) { }
    }
}