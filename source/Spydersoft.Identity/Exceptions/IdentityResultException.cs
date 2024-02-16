using System;
using System.Linq;

using Microsoft.AspNetCore.Identity;

namespace Spydersoft.Identity.Exceptions
{
    /// <summary>
    /// Class IdentityResultException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public class IdentityResultException(IdentityResult result) : Exception(result.Errors.First().Description)
    {
    }
}