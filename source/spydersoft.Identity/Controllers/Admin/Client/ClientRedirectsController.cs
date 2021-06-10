using AutoMapper.QueryableExtensions;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using spydersoft.Identity.Models.Admin.ClientViewModels;
using IS4Entities = Duende.IdentityServer.EntityFramework.Entities;

namespace spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientRedirectsController : BaseClientCollectionController<ClientRedirectViewModel, ClientRedirectsViewModel, IS4Entities.ClientRedirectUri>
    {
        public ClientRedirectsController(ConfigurationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region BaseClientCollectinoController Implementation

        protected override IEnumerable<ClientRedirectViewModel> PopulateItemList(IS4Entities.Client mainEntity)
        {
            return Mapper.ProjectTo<ClientRedirectViewModel>(mainEntity.RedirectUris.ToList().AsQueryable());
        }

        protected override IQueryable<IS4Entities.Client> AddIncludes(DbSet<IS4Entities.Client> query)
        {
            return query.Include(c => c.RedirectUris);
        }

        protected override IS4Entities.ClientRedirectUri FindItemInCollection(List<IS4Entities.ClientRedirectUri> collection, int id)
        {
            return collection.FirstOrDefault(r => r.Id == id);
        }

        protected override List<IS4Entities.ClientRedirectUri> GetCollection(IS4Entities.Client mainEntity)
        {
            return mainEntity.RedirectUris;
        }

        #endregion BaseClientCollectinoController Implementation
    }
}