using System;

namespace Spydersoft.Identity.Exceptions
{
    public class ObjectLoadException(string objectType, string objectId) : Exception($"Unable to load {objectType} with ID {objectId}")
    {
        public string ObjectId { get; } = objectId;
        public string ObjectType { get; } = objectType;
    }
}