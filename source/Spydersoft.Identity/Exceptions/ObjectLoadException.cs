using System;
using System.Runtime.Serialization;

namespace Spydersoft.Identity.Exceptions
{
    [Serializable]
    public class ObjectLoadException : Exception
    {
        public string ObjectId { get; }
        public string ObjectType { get; }

        protected ObjectLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        public ObjectLoadException(string objectType, string objectId) : base($"Unable to load {objectType} with ID {objectId}")
        {
            ObjectId = objectId;
            ObjectType = objectType;
        }
    }
}