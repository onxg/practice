namespace Practice.Repository
{
    using Practice.Core.Repositories;
    using Practice.DAL;
    using System;
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

        public async Task<ViewModels.Employee> GetEmployeeById(int id)
        {
            if (id == 0)
                return null;

            var employee = await context.vEmployee.FirstOrDefaultAsync(e => e.BusinessEntityID == id);
            if (employee == null)
                return null;

            return new ViewModels.Employee()
            {
                Id = employee.BusinessEntityID,
                Address = employee.AddressLine1,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                City = employee.City,
                PhoneNumber = employee.PhoneNumber,
                PostalCode = employee.PostalCode
            };
        }

        public async Task UpdateEmployee(ViewModels.Employee employee)
        {
            if (employee == null || employee.Id == 0)
                return;

            var person = await context.Person.FirstOrDefaultAsync(p => p.BusinessEntityID == employee.Id);
            if (person == null)
                return;

            // First and Last name
            person.FirstName = employee.FirstName;
            person.LastName = employee.LastName;

            // Phone
            var oldPhone = person.PersonPhone.FirstOrDefault();
            if (oldPhone != null)
            {
                person.PersonPhone.Remove(oldPhone);

                PersonPhone newPhone = new PersonPhone
                {
                    BusinessEntityID = oldPhone.BusinessEntityID,
                    Person = oldPhone.Person,
                    PhoneNumberType = oldPhone.PhoneNumberType,
                    PhoneNumberTypeID = oldPhone.PhoneNumberTypeID,
                    PhoneNumber = employee.PhoneNumber,
                    ModifiedDate = DateTime.Now
                };

                person.PersonPhone.Add(newPhone);
            }

            // Address
            var address = person.BusinessEntity.BusinessEntityAddress.FirstOrDefault()?.Address;
            if (address != null)
            {
                address.AddressLine1 = employee.Address;
                address.City = employee.City;
                address.PostalCode = employee.PostalCode;
            }

            await context.SaveChangesAsync();
        }
    }
}
