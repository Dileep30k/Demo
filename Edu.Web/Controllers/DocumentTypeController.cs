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
    public class DocumentTypeController : BaseController
    {
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IUserProviderService _userProviderService;

        public DocumentTypeController(
           IDocumentTypeService documentTypeService,
           IUserProviderService userProviderService
        )
        {
            _documentTypeService = documentTypeService;
            _userProviderService = userProviderService;
        }

        public async Task<IActionResult> Index()
        {
            if (!AuthorizeDocumentType()) { return AccessDeniedView(); }

            return View();
        }

        public async Task<IActionResult> Manage(long id = 0)
        {
            if (!AuthorizeDocumentType()) { return AccessDeniedView(); }

            var model = new DocumentType();
            if (id > 0)
            {
                var documentTypeResult = await _documentTypeService.GetDocumentTypeById(id);
                if (documentTypeResult.Success)
                {
                    model = documentTypeResult.Data;
                }
            }

            await SetComboBoxes(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(DocumentType model)
        {
            ResponseModel documentTypeResult;
            if (model.DocumentTypeId == 0)
            {
                documentTypeResult = await _documentTypeService.CreateDocumentType(model);
            }
            else
            {
                documentTypeResult = await _documentTypeService.UpdateDocumentType(model);
            }

            if (documentTypeResult.Success)
            {
                SetNotification($"Document Type saved successfully!", NotificationTypes.Success, "DocumentType");
                return RedirectToAction("Index", "DocumentType");
            }

            SetNotification(documentTypeResult.Message, NotificationTypes.Error, "DocumentType");
            await SetComboBoxes(model);
            return View(model);
        }

        public async Task<IActionResult> GetDocumentTypes()
        {
            try
            {
                dynamic filters = new ExpandoObject();
                dynamic grid = GetGridPagination(filters);
                var result = await _documentTypeService.GetDocumentTypePaged(grid.pagination);
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

        private async Task SetComboBoxes(DocumentType documentType)
        {
        }

        private bool AuthorizeDocumentType()
        {
            return _userProviderService.UserClaim.IsAdmin;
        }
    }
}
