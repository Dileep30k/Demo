using Core.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Edu.Web.Models;

namespace Edu.Web.Controllers
{
    public class LocationController : BaseController
    {
        private readonly ILocationService _locationService;
        private readonly IUserProviderService _userProviderService;

        public LocationController(
           ILocationService locationService,
           IUserProviderService userProviderService
        )
        {
            _locationService = locationService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeLocation()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeLocation()) { return AccessDeniedView(); }

            var model = new Location();
            if (id > 0)
            {
                var locationResult = await _locationService.GetLocationById(id);
                if (locationResult.Success)
                {
                    model = locationResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Location model)
        {
            ResponseModel locationResult;
            if (model.LocationId == 0)
            {
                locationResult = await _locationService.CreateLocation(model);
            }
            else
            {
                locationResult = await _locationService.UpdateLocation(model);
            }

            if (locationResult.Success)
            {
                SetNotification($"Location saved successfully!", NotificationTypes.Success, "Location");
                return RedirectToAction("Index", "Location");
            }

            SetNotification(locationResult.Message, NotificationTypes.Error, "Location");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetLocations()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _locationService.GetLocationPaged(grid.pagination);
                var paged = result.Data as PagedList;
                return Ok(new
                {
                    draw = grid.draw,
                    recordsFiltered = paged.TotalCount,
                    recordsTotal = paged.TotalCount,
                    data = paged.Data
                });
            }
            catch (Exception) { throw; }
        }

        private async Task SetComboBoxes(Location location)
        {
        }

        private bool AuthorizeLocation()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
