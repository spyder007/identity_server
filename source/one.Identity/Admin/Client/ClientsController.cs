using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using one.Identity.Controllers;
using one.Identity.Data;
using one.Identity.Quickstart;

namespace one.Identity.Admin.Client
{
    [Authorize(Roles ="admin")]
    public class ClientController : Controller
    {
        private ConfigurationDbContext _dbContext;
        private IMapper _mapper;

        public ClientController(ConfigurationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public IActionResult Index()
        {
            var clientsModel = new ClientsViewModel
            {
                Clients = _dbContext.Clients.ProjectTo<ClientViewModel>()
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
                var client = _dbContext.Clients.Include(c => c.AllowedScopes)
                    .FirstOrDefault(c => c.Id == id.Value);
                if (client == null)
                {
                    // TODO : Show error   
                    return RedirectToAction(nameof(HomeController.Error), nameof(HomeController), "Could not load client");
                }

                clientModel = new ClientViewModel(client.Id);
                _mapper.Map(client, clientModel);
            }

            return View(clientModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, ClientViewModel client)
        {
            if (id.HasValue)
            {
                client.Id = id.Value;
                var dbEntity = _dbContext.Clients.Include(c => c.AllowedScopes)
                    .FirstOrDefault(c => c.Id == id.Value);
                if (dbEntity != null)
                {
                    _mapper.Map(client, dbEntity);
                }

                _dbContext.Update(dbEntity);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }
    }
}
