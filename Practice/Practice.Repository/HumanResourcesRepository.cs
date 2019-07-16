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

        public async Task CreateEmployee(ViewModels.Employee employee)
        {
            BusinessEntity businessEntity = new BusinessEntity
            {
                rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now
            };

            //context.BusinessEntity.Add(businessEntity);


            Person newPerson = new Person
            {
                BusinessEntity = businessEntity,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PersonType = "EM",
                NameStyle = false,
                EmailPromotion = 0,
                rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now
            };

            PhoneNumberType phoneNumberType = await context.PhoneNumberType.FirstOrDefaultAsync(pnt => pnt.Name == "Cell");

            PersonPhone personPhone = new PersonPhone
            {
                PhoneNumberType = phoneNumberType,
                PhoneNumberTypeID = phoneNumberType.PhoneNumberTypeID,
                ModifiedDate = DateTime.Now,
                Person = newPerson,
                PhoneNumber = employee.PhoneNumber
            };

            newPerson.PersonPhone = new List<PersonPhone> { personPhone };

            context.Person.Add(newPerson);

            //int? maxId = (await context.Person.OrderByDescending(p => p.BusinessEntityID).Take(1).ToListAsync()).FirstOrDefault()?.BusinessEntityID;
            //if (maxId == null)
            //    maxId = 0;

            //Person newPerson = new Person
            //{
            //    BusinessEntityID = maxId.Value + 1,
            //    FirstName = employee.FirstName,
            //    LastName = employee.LastName
            //};
            //context.Person.Add(newPerson);


            //Employee newEmployee = new Employee
            //{
            //    Person = new Person
            //    {
            //        FirstName = employee.FirstName,
            //        LastName = employee.LastName,
            //        PersonPhone = new List<PersonPhone> { new PersonPhone { PhoneNumber = employee.PhoneNumber, PhoneNumberTypeID = 1 } },
            //    },
            //    CurrentFlag = true,

            //};

            //context.Employee.Add(newEmployee);
            await context.SaveChangesAsync();
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
