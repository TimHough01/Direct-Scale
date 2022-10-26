using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectScale.Disco.Extension.Middleware;
using DirectScale.Disco.Extension.Middleware.Models;
using TM3ClientExtension.Services;
using ClientExtension.Hooks.Autoships;

namespace TM3ClientExtension
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSingleton<IAssociateUpgradeService, AssociateUpgradeService>();

            services.AddDirectScale(options =>
            {
                // This Page and Page Link will show for all users in the DirectScale Platform
                options.AddCustomPage(Menu.Toolbar, "Hello John", "/CustomPage/HelloWorld?personsName=John");

                // This Page and Page Link will show for users that have the 'ViewAdministration' right in the DirectScale Platform
                // If the /CustomPage/SecuredHelloWorld page does not have an authorization attribute it can be accessed by anyone with the following URL
                // https://acme.clientextension.directscale<environment>.com/CustomPage/SecuredHelloWorld
                options.AddCustomPage(Menu.AssociateDetail, "Hello Associate", "ViewAdministration", "/CustomPage/SecuredHelloWorld");

                // Hooks
                // Below are some examples of how to register a Hook with the AddHook(string, string) method
                // This is an alternative way to register a hook with a controller. Important! Hooks can only be registered one way
                //options.AddHook("Autoships.CreateAutoship", "/api/hooks/AutoshipHooks/CreateAutoshipHook");
                //options.AddHook("Autoships.GetAutoships", "/api/hooks/AutoshipHooks/GetAutoshipsHook");
                //options.AddHook("Orders.SubmitOrder", "/api/hooks/OrderHooks/SubmitOrderHook");

                options.AddHook<CreateAutoshipHook>();
                options.AddHook<CancelAutoshipHook>();
                options.AddHook<UpdateAutoshipHook>();

                // WebHooks
                //options.AddEventHandler("OrderCreated", "/api/webhooks/Order/CreateOrder");

                // Merchants
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseDirectScale();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
