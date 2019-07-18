namespace Practice.App_Start
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Practice.DAL;
    using Practice.DAL.Identity;
    using Practice.Repository;
    using System.Linq;
    using System.Web.Mvc;

    public class AutofacConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<Context>().InstancePerRequest();
            builder.RegisterType<UserStore<ApplicationUser>>().As<IUserStore<ApplicationUser>>();
            builder.RegisterType<UserManager<ApplicationUser>>();
            builder.RegisterAssemblyTypes(typeof(HumanResourcesRepository).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerRequest();
            builder.RegisterFilterProvider();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}