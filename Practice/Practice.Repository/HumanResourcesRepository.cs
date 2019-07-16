namespace Practice.Repository
{
    using Practice.Core.Repositories;
    using Practice.DAL;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels = Practice.Core.ViewModels;

    public class HumanResourcesRepository : IHumanResourcesRepository
    {
        private readonly Context context;

        public HumanResourcesRepository()
        {
            context = new Context();
        }

        public async Task<List<ViewModels.Employee>> GetEmployees()
        {
            var employees = await context.vEmployee
                            .Where(e => context.Employee.FirstOrDefault(y => y.BusinessEntityID == e.BusinessEntityID).CurrentFlag == true)
                            .Select(x => new ViewModels.Employee()
                            {
                                Id = x.BusinessEntityID,
                                Address = x.AddressLine1,
                                FirstName = x.FirstName,
                                LastName = x.LastName,
                                City = x.City,
                                PhoneNumber = x.PhoneNumber,
                                PostalCode = x.PostalCode
                            }).ToListAsync();

            return employees;
        }

        public async Task DeleteEmployee(int id)
        {
            if (id == 0)
            {
                return;
            }

            Employee employee = await context.Employee.FirstOrDefaultAsync(x => x.BusinessEntityID == id);

            if (employee == null)
            {
                return;
            }

            employee.CurrentFlag = false;

            await context.SaveChangesAsync();
        }
    }
}
