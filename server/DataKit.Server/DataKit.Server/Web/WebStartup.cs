using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace DataKit.Server.Web
{
    public class WebStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());
        }
    }
}