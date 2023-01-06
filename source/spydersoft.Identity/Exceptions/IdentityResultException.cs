using System;
using System.Linq;

using Microsoft.AspNetCore.Identity;

namespace spydersoft.Identity.Exceptions
{
    public class IdentityResultException : Exception
    {
        public IdentityResultException(IdentityResult result) : base(result.Errors.First().Description)
        {

        }
    }
}