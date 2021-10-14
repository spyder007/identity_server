// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


namespace spydersoft.Identity.Models.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}