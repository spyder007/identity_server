// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Spydersoft.Identity.Models.Consent;

namespace Spydersoft.Identity.Models.Device
{
    /// <summary>
    /// Class DeviceAuthorizationInputModel.
    /// Implements the <see cref="ConsentInputModel" />
    /// </summary>
    /// <seealso cref="ConsentInputModel" />
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        /// <value>The user code.</value>
        public string UserCode { get; set; }
    }
}