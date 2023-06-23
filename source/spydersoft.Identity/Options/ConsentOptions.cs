// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Spydersoft.Identity.Options
{
    public class ConsentOptions
    {
        public const string SettingsKey = "Consent";

        public bool EnableOfflineAccess { get; set; } = true;
        public string OfflineAccessDisplayName { get; set; } = "Offline Access";
        public string OfflineAccessDescription { get; set; } = "Access to your applications and resources, even when you are offline";

        public string MustChooseOneErrorMessage { get; set; } = "You must pick at least one permission";
        public string InvalidSelectionErrorMessage { get; set; } = "Invalid selection";
    }
}