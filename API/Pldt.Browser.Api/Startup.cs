using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Pldt.Browser.Api.Startup))]
namespace Pldt.Browser.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
