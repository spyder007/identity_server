// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Spydersoft.Identity.Models.AccountViewModels
{
    /// <summary>
    /// Class LogoutViewModel.
    /// Implements the <see cref="LogoutInputModel" />
    /// </summary>
    /// <seealso cref="LogoutInputModel" />
    public class LogoutViewModel : LogoutInputModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether [show logout prompt].
        /// </summary>
        /// <value><c>true</c> if [show logout prompt]; otherwise, <c>false</c>.</value>
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}