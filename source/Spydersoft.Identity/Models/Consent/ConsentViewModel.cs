// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Spydersoft.Identity.Models.Consent
{
    /// <summary>
    /// Class ConsentViewModel.
    /// Implements the <see cref="ConsentInputModel" />
    /// </summary>
    /// <seealso cref="ConsentInputModel" />
    public class ConsentViewModel : ConsentInputModel
    {
        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>The name of the client.</value>
        public string ClientName { get; set; }
        /// <summary>
        /// Gets or sets the client URL.
        /// </summary>
        /// <value>The client URL.</value>
        public string ClientUrl { get; set; }
        /// <summary>
        /// Gets or sets the client logo URL.
        /// </summary>
        /// <value>The client logo URL.</value>
        public string ClientLogoUrl { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [allow remember consent].
        /// </summary>
        /// <value><c>true</c> if [allow remember consent]; otherwise, <c>false</c>.</value>
        public bool AllowRememberConsent { get; set; }

        /// <summary>
        /// Gets or sets the identity scopes.
        /// </summary>
        /// <value>The identity scopes.</value>
        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        /// <summary>
        /// Gets or sets the API scopes.
        /// </summary>
        /// <value>The API scopes.</value>
        public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
    }
}