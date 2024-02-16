// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Spydersoft.Identity.Models.Consent
{
    /// <summary>
    /// Class ScopeViewModel.
    /// </summary>
    public class ScopeViewModel
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScopeViewModel"/> is emphasize.
        /// </summary>
        /// <value><c>true</c> if emphasize; otherwise, <c>false</c>.</value>
        public bool Emphasize { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScopeViewModel"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScopeViewModel"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked { get; set; }
    }
}