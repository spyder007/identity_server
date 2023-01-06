using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.AspNetCore.Mvc;

using spydersoft.Identity.Models.Admin.ClientViewModels;

namespace spydersoft.Identity.Controllers.Admin.Client
{
    public class ClientsController : BaseAdminController
    {
        public ClientsController(ConfigurationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        #region Client List Actions

        public IActionResult Index()
        {
            ViewData["Title"] = "Registered Clients";

            var clientsModel = new ClientsViewModel
            {
                Clients = Mapper.ProjectTo<ClientViewModel>(ConfigDbContext.Clients.AsQueryable())
            };

            return View(clientsModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue)
            {
                Duende.IdentityServer.EntityFramework.Entities.Client client = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                _ = ConfigDbContext.Clients.Remove(client);
                _ = await ConfigDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion Client List Actions

        #region Main Tab
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ClientViewModel clientModel;
            if (!id.HasValue)
            {
                clientModel = new ClientViewModel();
                ViewData["Title"] = "New Client";
            }
            else
            {
                Duende.IdentityServer.EntityFramework.Entities.Client client = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    return GetErrorAction("Could not load client");
                }

                clientModel = new ClientViewModel(client.Id);
                _ = Mapper.Map(client, clientModel);
                ViewData["Title"] = $"Edit {clientModel.NavBar.Name}";
            }

            return View(clientModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ClientViewModel client)
        {
            if (ModelState.IsValid)
            {
                Duende.IdentityServer.EntityFramework.Entities.Client dbEntity;
                var isNew = false;

                if (!id.HasValue || id.Value == 0)
                {
                    dbEntity = new Duende.IdentityServer.EntityFramework.Entities.Client();
                    _ = ConfigDbContext.Add(dbEntity);
                    isNew = true;
                }
                else
                {
                    client.Id = id.Value;
                    dbEntity = ConfigDbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                }

                if (dbEntity != null)
                {
                    _ = Mapper.Map(client, dbEntity);
                }

                if (!isNew)
                {
                    _ = ConfigDbContext.Update(dbEntity);
                }

                _ = await ConfigDbContext.SaveChangesAsync();

                return isNew ? RedirectToAction(nameof(Edit), new { id = dbEntity.Id }) : RedirectToAction(nameof(Index));
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