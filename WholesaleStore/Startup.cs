using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WholesaleStore.Startup))]

namespace WholesaleStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}