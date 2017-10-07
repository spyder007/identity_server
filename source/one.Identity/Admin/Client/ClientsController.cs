using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using one.Identity.Data;

namespace one.Identity.Admin.Client
{
    [Authorize]
    [Route("[controller]/[action]")]
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
                Clients = _dbContext.Clients.Select(
                    client => new ClientViewModel
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName
                    })
            };


            return View(clientsModel);
        }
    }
}
