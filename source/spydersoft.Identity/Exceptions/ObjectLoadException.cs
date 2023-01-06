using System;

namespace spydersoft.Identity.Exceptions
{
    public class ObjectLoadException : Exception
    {
        public string ObjectId { get; }
        public string ObjectType { get; }

        public ObjectLoadException(string objectType, string objectId) : base($"Unable to load {objectType} with ID {objectId}")
        {
            ObjectId = objectId;
            ObjectType = objectType;
        }
    }
}