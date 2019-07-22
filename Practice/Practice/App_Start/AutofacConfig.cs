using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Practice.Core.Services;
using Practice.DAL;
using Practice.DAL.Identity;
using Practice.Repository;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Practice.App_Start
{
    public class AutofacConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            // Context, user storage and auth services
            builder.RegisterType<Context>().InstancePerRequest();
            builder.Register(c => new UserStore<ApplicationUser>(c.Resolve<Context>())).As<IUserStore<ApplicationUser, string>>();
            builder.Register(c => new DataProtectorTokenProvider<ApplicationUser>(new DpapiDataProtectionProvider().Create("ASP.NET Identity"))).As<IUserTokenProvider<ApplicationUser, string>>().SingleInstance();

            builder.Register(c => new UserManager<ApplicationUser, string>(c.Resolve<IUserStore<ApplicationUser, string>>())
            {
                UserTokenProvider = c.Resolve<IUserTokenProvider<ApplicationUser, string>>(),
                EmailService = c.Resolve<IIdentityMessageService>()
            });
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.RegisterType<SignInManager<ApplicationUser, string>>();

            // Repositories
            builder.RegisterAssemblyTypes(typeof(HumanResourcesRepository).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            // Services
            var appSettings = WebConfigurationManager.AppSettings;
            builder.Register(c => new EmailService(new SmtpClient(appSettings["mailHost"], int.Parse(appSettings["mailPort"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(appSettings["mailAccount"], appSettings["mailPassword"])
            })).As<IIdentityMessageService>();

            // Controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterFilterProvider();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}