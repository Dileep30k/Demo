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
using Edu.Service.Services;

namespace Edu.Web.Controllers
{
    public class BatchController : BaseController
    {
        private readonly IBatchService _batchService;
        private readonly IUserProviderService _userProviderService;

        public BatchController(
           IBatchService batchService,
           IUserProviderService userProviderService
        )
        {
            _batchService = batchService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeBatch()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeBatch()) { return AccessDeniedView(); }

            var model = new Batch();
            if (id > 0)
            {
                var BatchResult = await _batchService.GetBatchById(id);
                if (BatchResult.Success)
                {
                    model = BatchResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Batch model)
        {
            ResponseModel BatchResult;
            if (model.BatchId == 0)
            {
                BatchResult = await _batchService.CreateBatch(model);
            }
            else
            {
                BatchResult = await _batchService.UpdateBatch(model);
            }

            if (BatchResult.Success)
            {
                SetNotification($"Batch saved successfully!", NotificationTypes.Success, "Batch");
                return RedirectToAction("Index", "Batch");
            }

            SetNotification(BatchResult.Message, NotificationTypes.Error, "Batch");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetBatches(
            long? instituteId = null
        )
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                filters.instituteId = instituteId;
                var result = await _batchService.GetBatchPaged(grid.pagination);
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

        private async Task SetComboBoxes(Batch Batch)
        {
        }

        private bool AuthorizeBatch()
        {
            return _userProviderService.UserClaim.IsGts;
        }
    }
}
