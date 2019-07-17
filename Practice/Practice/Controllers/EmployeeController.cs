namespace Practice.Controllers
{
    using Practice.Core.Repositories;
    using System;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class EmployeeController : Controller
    {
        private readonly IHumanResourcesRepository repository;

        public EmployeeController(IHumanResourcesRepository _repository)
        {
            repository = _repository;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> GetEmployeesData()
        {
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string serchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            var model = await repository.GetEmployees();
            int totalRows = model.Count;
            if (!string.IsNullOrEmpty(serchValue))
            {
                model = model.Where(x => x.LastName.ToLower().Contains(serchValue.ToLower())|| 
                 x.FirstName.ToLower().Contains(serchValue.ToLower()) ||
                 x.PhoneNumber.ToLower().Contains(serchValue.ToLower()) ||
                 x.Address.ToLower().Contains(serchValue.ToLower()) ||
                 x.PostalCode.ToLower().Contains(serchValue.ToLower()) ||
                 x.City.ToLower().Contains(serchValue.ToLower()) ||
                 x.Id.ToString().Contains(serchValue.ToLower()) ).ToList();
            }

              model = model.OrderBy(sortColumnName+" "+sortDirection).ToList();

            int totalRowsAfterFiltering = model.Count;
            model =  model.Skip(start).Take(length).ToList();
            return Json(new { data = model,draw=Request["draw"],recordsTotal=totalRows,recordsFiltered= totalRowsAfterFiltering, JsonRequestBehavior.AllowGet });
        }
        
        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            await repository.DeleteEmployee(id.Value);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue || id == 0)
                return RedirectToAction("Index");

            var model = await repository.GetEmployeeById(id.Value);
            if(model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Core.ViewModels.Employee employee)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = employee.Id });

            await repository.UpdateEmployee(employee);
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Details(int? id)
        {
            if (!id.HasValue || id == 0)
                return RedirectToAction("Index");

            var model = await repository.GetEmployeeById(id.Value);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}