using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using one.Identity.Controllers;
using one.Identity.Data;
using one.Identity.Quickstart;

namespace one.Identity.Admin.Client
{
    [Authorize]
    public class ClientController : Controller
    {
        private ConfigurationDbContext _dbContext;

        public ClientController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IActionResult Index()
        {
            var clientsModel = new ClientsViewModel
            {
                Clients = _dbContext.Clients.Select(client => new ClientViewModel(client))
            };


            return View(clientsModel);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ClientViewModel clientModel;
            if (!id.HasValue)
            {
                clientModel = new ClientViewModel(new IdentityServer4.EntityFramework.Entities.Client());
            }
            else
            {
                var client = _dbContext.Clients.FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    // TODO : Show error   
                    return RedirectToAction(nameof(HomeController.Error), nameof(HomeController), "Could not load client");
                }

                clientModel = new ClientViewModel(client);
            }

            return View(clientModel);
        }
    }
}
