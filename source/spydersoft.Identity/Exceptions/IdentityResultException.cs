using System;
using System.Linq;
using System.Runtime.Serialization;

using Microsoft.AspNetCore.Identity;

namespace spydersoft.Identity.Exceptions
{
    [Serializable]
    public class IdentityResultException : Exception
    {
        protected IdentityResultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        public IdentityResultException(IdentityResult result) : base(result.Errors.First().Description)
        {

        }
    }
}