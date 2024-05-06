using Core.Repository.Models;
using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Enums;
using Edu.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Edu.Web.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
    [Authorize]
    [CustomAuthorize]
    public class BaseController : Controller
    {
        protected ActionResult AccessDeniedView()
        {
            return RedirectToAction("AccessDenied", "Dashboard");
        }

        protected void SetNotification(string message, NotificationTypes type, string title)
        {
            TempData["Notification"] = JsonConvert.SerializeObject(new AlertNotificationModel
            {
                Message = message,
                Title = title,
                Type = type
            });
        }

        public List<SelectListItem> GetSelectList(List<DropdownModel> items, string defaultText = "", List<long> selected = null)
        {
            var list = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(defaultText))
            {
                list.Add(new SelectListItem()
                {
                    Text = defaultText,
                    Value = "",
                    Selected = true
                });
            }

            foreach (var item in items)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Text,
                    Value = item.Id.ToString(),
                    Selected = selected != null ? selected.IndexOf(item.Id) != -1 : false
                });
            }

            return list;
        }

        public dynamic GetGridPagination(dynamic filters)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int currentPage = skip / Convert.ToInt32(length) + 1;

            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            filters.searchText = searchValue;

            return new
            {
                draw,
                pagination = new Pagination
                {
                    PageNumber = currentPage,
                    PageSize = pageSize,
                    SortOrderBy = sortColumnDirection,
                    SortOrderColumn = sortColumn,
                    Filters = filters
                }
            };
        }
    }
}
