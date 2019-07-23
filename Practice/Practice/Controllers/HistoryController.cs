using Practice.Core.Repositories;
using Practice.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Practice.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IHistoryRepository historyRepository;

        public HistoryController(IHistoryRepository _historyRepository)
        {
            historyRepository = _historyRepository;
        }


        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetAllHistoriesAsync(FormCollection form)
        {
            var searchFilters = new SearchFilters(form)
            {
                OrderBy = GetOrderBy(form)
            };

            var result = await historyRepository.GetAllHistoriesAsync(searchFilters);

            return this.Json(
                new
                {
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                }, JsonRequestBehavior.AllowGet);
        }

        private string GetOrderBy(FormCollection form)
        {
            string orderBy = form["order[0][column]"];

            switch (orderBy)
            {
                case "0":
                    orderBy = "Id";
                    break;
                case "1":
                    orderBy = "FirstName";
                    break;
                case "2":
                    orderBy = "LastName";
                    break;
                case "3":
                    orderBy = "Department";
                    break;
                case "4":
                    orderBy = "StartDate";
                    break;
                default:
                    orderBy = "Id";
                    break;
            }

            orderBy = (orderBy + " " + form["order[0][dir]"]).ToUpper();

            return orderBy;
        }


        public async Task<ActionResult> GetHistory(FormCollection form)
        {
            if (!int.TryParse(form["id"], out int id) || id == 0)
                return Json(new { status = "error", message = "Invalid id." });

            DateTime date = Convert.ToDateTime(form["startDate"]);

            var history = await historyRepository.GetHistory(id, date, form["department"]);

            return Json(new { status = "success", history });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(FormCollection form)
        {
            string idStr = form["id"];
            if (!int.TryParse(form["id"], out int id) || id == 0)
                return Json(new { status = "error", message = "Invalid id." });

            DateTime date = Convert.ToDateTime(form["startDate"]);

            try
            {
                await historyRepository.Delete(id, date, form["department"]);
            }
            catch (InvalidOperationException e)
            {
                return Json(new { status = "error", message = e.Message });
            }

            return Json(new { status = "success", message = "History has been successfully deleted." });
        }
    }
}