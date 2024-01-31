using System;
using System.Linq;

using Microsoft.AspNetCore.Identity;

namespace Spydersoft.Identity.Exceptions
{
    public class IdentityResultException(IdentityResult result) : Exception(result.Errors.First().Description)
    {
    }
}