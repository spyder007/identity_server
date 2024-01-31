// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Spydersoft.Identity.Models.Consent;

namespace Spydersoft.Identity.Models.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}