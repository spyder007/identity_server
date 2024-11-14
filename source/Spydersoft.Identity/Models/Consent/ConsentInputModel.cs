// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Spydersoft.Identity.Models.Consent
{
    /// <summary>
    /// Class ConsentInputModel.
    /// </summary>
    public class ConsentInputModel
    {
        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        /// <value>The button.</value>
        public string Button { get; set; }
        /// <summary>
        /// Gets or sets the scopes consented.
        /// </summary>
        /// <value>The scopes consented.</value>
        public IEnumerable<string> ScopesConsented { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [remember consent].
        /// </summary>
        /// <value><c>true</c> if [remember consent]; otherwise, <c>false</c>.</value>
        public bool RememberConsent { get; set; } = false;
        /// <summary>
        /// Gets or sets the return URL.
        /// </summary>
        /// <value>The return URL.</value>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
    }
}