// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Spydersoft.Identity.Options
{
    /// <summary>
    /// Class ConsentOptions.
    /// </summary>
    public class ConsentOptions
    {
        /// <summary>
        /// The settings key
        /// </summary>
        public const string SettingsKey = "Consent";

        /// <summary>
        /// Gets or sets a value indicating whether [enable offline access].
        /// </summary>
        /// <value><c>true</c> if [enable offline access]; otherwise, <c>false</c>.</value>
        public bool EnableOfflineAccess { get; set; } = true;
        /// <summary>
        /// Gets or sets the display name of the offline access.
        /// </summary>
        /// <value>The display name of the offline access.</value>
        public string OfflineAccessDisplayName { get; set; } = "Offline Access";
        /// <summary>
        /// Gets or sets the offline access description.
        /// </summary>
        /// <value>The offline access description.</value>
        public string OfflineAccessDescription { get; set; } = "Access to your applications and resources, even when you are offline";

        /// <summary>
        /// Gets or sets the must choose one error message.
        /// </summary>
        /// <value>The must choose one error message.</value>
        public string MustChooseOneErrorMessage { get; set; } = "You must pick at least one permission";
        /// <summary>
        /// Gets or sets the invalid selection error message.
        /// </summary>
        /// <value>The invalid selection error message.</value>
        public string InvalidSelectionErrorMessage { get; set; } = "Invalid selection";
    }
}