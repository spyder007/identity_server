using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using one.Identity.Models.ClientViewModels;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;

namespace one.Identity.Controllers.Admin.Client
{
    public class ClientController : BaseAdminController
    {
        public ClientController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

        #region Client List Actions

        public IActionResult Index()
        {
            var clientsModel = new ClientsViewModel
            {
                Clients = ConfigDbContext.Clients.ProjectTo<ClientViewModel>()
            };

            return View(clientsModel);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ClientViewModel clientModel;
            if (!id.HasValue)
            {
                clientModel = new ClientViewModel();
            }
            else
            {
                var client = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                clientModel = new ClientViewModel(client.Id);
                Mapper.Map(client, clientModel);
            }

            return View(clientModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue)
            {
                var client = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                ConfigDbContext.Clients.Remove(client);
                await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Client List Actions

        #region Main Tab

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ClientViewModel client)
        {
            if (ModelState.IsValid)
            {
                IdentityServer4.EntityFramework.Entities.Client dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new IdentityServer4.EntityFramework.Entities.Client();
                    ConfigDbContext.Add(dbEntity);
                    isNew = true;
                }
                else
                {
                    client.Id = id.Value;
                    dbEntity = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                }

                if (dbEntity != null)
                {
                    Mapper.Map(client, dbEntity);
                }

                if (!isNew)
                {
                    ConfigDbContext.Update(dbEntity);
                }

                await ConfigDbContext.SaveChangesAsync();

                return (isNew ? RedirectToAction(nameof(Edit), new { id = dbEntity.Id }) : RedirectToAction(nameof(Index)));
            }

            if (id.HasValue)
            {
                client.Id = id.Value;
            }

            return View(client);
        }

        #endregion Main Tab
    }
}