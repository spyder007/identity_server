using System;

namespace Spydersoft.Identity.Exceptions
{
    public class IdentityServerException : Exception
    {
        public IdentityServerException() { }

        public IdentityServerException(string message) : base(message) { }
    }
}