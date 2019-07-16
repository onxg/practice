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
            var employees = await context.vEmployee.Select(x => new ViewModels.Employee()
            {
                Address = x.AddressLine1,
                FirstName = x.FirstName,
                LastName = x.LastName,
                City = x.City,
                PhoneNumber = x.PhoneNumber,
                PostalCode = x.PostalCode
            }).ToListAsync();

            return employees;
        }
    }
}
