using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace one.Identity.Admin.Client
{
    public class ClientViewModel
    {
        private readonly IdentityServer4.EntityFramework.Entities.Client _entity;

        public ClientViewModel(IdentityServer4.EntityFramework.Entities.Client clientEntity)
        {
            _entity = clientEntity;
        }

        public int Id => _entity.Id;

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [Display(Name = "Client ID")]
        public string ClientId
        {
            get => _entity.ClientId;
            set => _entity.ClientId = value;
        }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Client Name")]
        public string ClientName
        {
            get => _entity.ClientName;
            set => _entity.ClientName = value;
        }

        public IdentityServer4.EntityFramework.Entities.Client Entity => _entity;

    }
}
