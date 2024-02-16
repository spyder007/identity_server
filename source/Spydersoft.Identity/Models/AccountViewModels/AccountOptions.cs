// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
namespace Spydersoft.Identity.Models.AccountViewModels
{
    /// <summary>
    /// Class AccountOptions.
    /// </summary>
    public static class AccountOptions
    {
        /// <summary>
        /// The allow local login
        /// </summary>
        public static readonly bool AllowLocalLogin = true;
        /// <summary>
        /// The allow remember login
        /// </summary>
        public static readonly bool AllowRememberLogin = true;
        /// <summary>
        /// The remember me login duration
        /// </summary>
        public static readonly TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        /// <summary>
        /// The show logout prompt
        /// </summary>
        public static readonly bool ShowLogoutPrompt = true;
        /// <summary>
        /// The automatic redirect after sign out
        /// </summary>
        public static readonly bool AutomaticRedirectAfterSignOut = false;

        /// <summary>
        /// The invalid credentials error message
        /// </summary>
        public static readonly string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}