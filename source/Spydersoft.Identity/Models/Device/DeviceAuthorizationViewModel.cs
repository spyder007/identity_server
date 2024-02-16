// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Spydersoft.Identity.Models.Consent;

namespace Spydersoft.Identity.Models.Device
{
    /// <summary>
    /// Class DeviceAuthorizationViewModel.
    /// Implements the <see cref="ConsentViewModel" />
    /// </summary>
    /// <seealso cref="ConsentViewModel" />
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        /// <value>The user code.</value>
        public string UserCode { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [confirm user code].
        /// </summary>
        /// <value><c>true</c> if [confirm user code]; otherwise, <c>false</c>.</value>
        public bool ConfirmUserCode { get; set; }
    }
}