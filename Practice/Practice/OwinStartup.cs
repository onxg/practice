using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Practice.OwinStartup))]

namespace Practice
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/Practice"),
                CookieName = "PracticeAuthentication",
                CookieHttpOnly = true,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}
