using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;

using Microsoft.EntityFrameworkCore;

using Spydersoft.Identity.Models.Admin.ClientViewModels;

namespace Spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientSecretsController(ConfigurationDbContext context, IMapper mapper) : BaseClientCollectionController<ClientSecretViewModel, ClientSecretsViewModel, ClientSecret>(context, mapper)
    {

        #region BaseClientCollectionController Implementation

        protected override IEnumerable<ClientSecretViewModel> PopulateItemList(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientSecretViewModel>(mainEntity.ClientSecrets.AsQueryable());
        }

        protected override IQueryable<Duende.IdentityServer.EntityFramework.Entities.Client> AddIncludes(
            DbSet<Duende.IdentityServer.EntityFramework.Entities.Client> query)
        {
            return query.Include(c => c.ClientSecrets);
        }

        protected override ClientSecret FindItemInCollection(List<ClientSecret> collection, int id)
        {
            return collection.Find(s => s.Id == id);
        }

        protected override List<ClientSecret> GetCollection(Duende.IdentityServer.EntityFramework.Entities.Client mainEntity)
        {
            return mainEntity.ClientSecrets;
        }

        protected override void SetAdditionalProperties(ClientSecret newItem)
        {
            base.SetAdditionalProperties(newItem);
            newItem.Created = DateTime.UtcNow;
            newItem.Value = newItem.Value.Sha256();
        }

        #endregion BaseClientCollectionController Implementation
    }
}