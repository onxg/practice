namespace Practice.Repository
{
    using Practice.Core.Repositories;
    using Practice.DAL;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Threading.Tasks;
    using ViewModels = Practice.Core.ViewModels;

    public class HumanResourcesRepository : IHumanResourcesRepository
    {
        private readonly Context context;

        public HumanResourcesRepository()
        {
            context = new Context();
        }
        public async Task<ViewModels.DataTablesObject<ViewModels.Employee>> GetEmployeesAsync(ViewModels.SearchFilters searchFilters)
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

            if (!string.IsNullOrEmpty(searchFilters.SearchValue))
            {
                var keyword = searchFilters.SearchValue.ToLower().Trim();
                employees = employees
                    .Where(e => e.FirstName.ToString().Contains(keyword)
                    || e.LastName.Contains(keyword)
                    || e.PhoneNumber.Contains(keyword)
                    || e.Address.Contains(keyword)
                    || e.PostalCode.Contains(keyword)
                    || e.City.Contains(keyword)
                    )
                    .ToList();
            }

            var rawData = employees
              .OrderBy(searchFilters.OrderBy)
              .Skip(searchFilters.DisplayStart)
              .Take(searchFilters.DisplayLength)
              .ToList();



            return new ViewModels.DataTablesObject<ViewModels.Employee>
            {
                aaData = rawData,
                iTotalDisplayRecords = employees.Count,
                iTotalRecords = employees.Count
            };

        }

        public async Task<ViewModels.Employee> GetEmployeeById(int id)
        {
            if (id == 0)
                return null;

            var employee = await context.vEmployee.SingleAsync(e => e.BusinessEntityID == id);

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

        public async Task CreateEmployee(ViewModels.Employee employee)
        {
            // Check if employee exists in the db
            if (await EmployeeExists(employee))
                throw new InvalidOperationException("This employee already exists.");

            // Create a person entity with all needed relations
            var person = new Person
            {
                BusinessEntity = new BusinessEntity
                {
                    rowguid = Guid.NewGuid(),
                    ModifiedDate = DateTime.Now,
                    BusinessEntityAddress = new List<BusinessEntityAddress>
                    {
                        new BusinessEntityAddress
                        {
                            Address = new Address
                            {
                                AddressLine1 = employee.Address,
                                AddressLine2 = Guid.NewGuid().ToString(), // to avoid bug with 2 different employees having the same address
                                City = employee.City,
                                PostalCode = employee.PostalCode,
                                rowguid = Guid.NewGuid(),
                                ModifiedDate = DateTime.Now,
                                StateProvince = await context.StateProvince.FirstOrDefaultAsync()
                            },
                            AddressType = await context.AddressType.FirstOrDefaultAsync(at => at.Name == "Home"),
                            ModifiedDate = DateTime.Now,
                            rowguid = Guid.NewGuid()
                        }
                    }
                },
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PersonType = "EM",
                NameStyle = false,
                EmailPromotion = 0,
                rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now,
                PersonPhone = new List<PersonPhone> {
                    new PersonPhone
                    {
                        PhoneNumberType = await context.PhoneNumberType.FirstOrDefaultAsync(pnt => pnt.Name == "Cell"),
                        ModifiedDate = DateTime.Now,
                        PhoneNumber = employee.PhoneNumber
                    }
                }
            };

            // Create new employee with random properties
            var random = new Random();
            var newEmployee = new Employee
            {
                Person = person,
                NationalIDNumber = random.Next().ToString(),
                LoginID = $"adventure-works/{employee.FirstName.ToLower() + random.Next().ToString()}",
                JobTitle = "Intern",
                BirthDate = new DateTime(random.Next(1970, 2000), random.Next(1, 12), random.Next(1, 28)),
                MaritalStatus = "S",
                Gender = random.Next(0, 1) == 1 ? "M" : "F",
                HireDate = DateTime.Now - TimeSpan.FromDays(1),
                SalariedFlag = false,
                VacationHours = 25,
                SickLeaveHours = 0,
                CurrentFlag = true,
                rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now
            };

            context.Employee.Add(newEmployee);
            await context.SaveChangesAsync();
        }

        public async Task DeleteEmployee(int id)
        {
            if (id == 0)
                return;

            var employee = await context.Employee.SingleAsync(x => x.BusinessEntityID == id);

            employee.CurrentFlag = false;

            await context.SaveChangesAsync();
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
                // PersonPhone Primary Key = BusinessEntityID + PhoneNumber + PhoneNumberTypeID 
                // so PhoneNumber can't be modified then it's removed and added again
                person.PersonPhone.Remove(oldPhone);

                var newPhone = new PersonPhone
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

        #region Private helper methods

        private async Task<bool> EmployeeExists(ViewModels.Employee employee)
        {
            var existingEmployee = await context.vEmployee.FirstOrDefaultAsync(ve => ve.FirstName == employee.FirstName &&
                                                                                     ve.LastName == employee.LastName &&
                                                                                     ve.PhoneNumber == employee.PhoneNumber &&
                                                                                     ve.AddressLine1 == employee.Address &&
                                                                                     ve.PostalCode == employee.PostalCode &&
                                                                                     ve.City == employee.City);

            return existingEmployee != null;
        }
        #endregion
    }
}
