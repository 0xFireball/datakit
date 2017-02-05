using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace DataKit.Server.Web
{
    public class WebStartup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseOwin(x => x.UseNancy(options => options.PassThroughWhenStatusCodesAre(
                HttpStatusCode.NotFound,
                HttpStatusCode.InternalServerError
            )));
            // app.UseWebSockets();
        }
    }
}