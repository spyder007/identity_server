// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using spydersoft.Identity.Models.Consent;

namespace spydersoft.Identity.Models.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}