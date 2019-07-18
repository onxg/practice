namespace Practice.Controllers
{
    using Practice.Core.Repositories;
    using Practice.Core.ViewModels;
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class EmployeeController : Controller
    {
        private readonly IHumanResourcesRepository repository;

        public EmployeeController(IHumanResourcesRepository _repository)
        {
            repository = _repository;
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetAllEmployees(FormCollection form)
        {
            var searchFilters = new SearchFilters(form)
            {
                OrderBy = GetOrderBy(form)
            };

            var result = await repository.GetEmployeesAsync(searchFilters);

            return this.Json(
                new
                {
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection formCollection)
        {
            Core.ViewModels.Employee newEmployee = new Core.ViewModels.Employee
            {
                FirstName = formCollection["firstName"],
                LastName = formCollection["lastName"],
                PhoneNumber = formCollection["phoneNumber"],
                Address = formCollection["address"],
                PostalCode = formCollection["postalCode"],
                City = formCollection["city"]
            };
            
            try
            {
                await repository.CreateEmployee(newEmployee);
            }
            catch (InvalidOperationException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
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
            if (model == null)
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

        private string GetOrderBy(FormCollection form)
        {
            string orderBy = form["order[0][column]"];

            switch (orderBy)
            {
                case "0":
                default:
                    orderBy = "FirstName";
                    break;
                case "1":
                    orderBy = "LastName";
                    break;
                case "2":
                    orderBy = "PhoneNumber";
                    break;
                case "3":
                    orderBy = "Address";
                    break;
                case "4":
                    orderBy = "PostalCode";
                    break;
                case "5":
                    orderBy = "City";
                    break;
            }

            orderBy = (orderBy + " " + form["order[0][dir]"]).ToUpper();

            return orderBy;
        }
    }
}