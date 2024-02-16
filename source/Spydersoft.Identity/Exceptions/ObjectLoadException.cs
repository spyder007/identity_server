using System;

namespace Spydersoft.Identity.Exceptions
{
    /// <summary>
    /// Class ObjectLoadException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public class ObjectLoadException(string objectType, string objectId) : Exception($"Unable to load {objectType} with ID {objectId}")
    {
        /// <summary>
        /// Gets the object identifier.
        /// </summary>
        /// <value>The object identifier.</value>
        public string ObjectId { get; } = objectId;
        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        public string ObjectType { get; } = objectType;
    }
}