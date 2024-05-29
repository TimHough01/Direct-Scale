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
using TM3ClientExtension.Repositories;
using TM3ClientExtension.Services.DailyRun;
using TM3ClientExtension.ThirdParty.ZiplingoEngagement.Interfaces;
using TM3ClientExtension.ThirdParty.ZiplingoEngagement;
using TM3ClientExtension.Hooks.Associate;
using TM3ClientExtension.Hooks.Order;
using TM3ClientExtension.Hooks;
using WebExtension.Repositories;
using WebExtension.Services;

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

            //services.AddSingleton<ITokenProvider>(x => new WebExtensionTokenProvider
            //{
            //    DirectScaleUrl = "https://tm3united.corpadmin.directscalestage.com",
            //    DirectScaleSecret = "1WaL4NwFUVqJ9E0FnpyJ4oklT9LiL-JFMAk1IClBxLXf",
            //    ExtensionSecrets = new[] { "95AglyMuoMf9m-TVoXpdwDS3ak2Lb9H4cWPtk2T0XCHG" }
            //});

            services.AddControllersWithViews();
            services.AddSingleton<IAssociateUpgradeService, AssociateUpgradeService>();

            //Repositories
            services.AddSingleton<ICustomLogRepository, CustomLogRepository>();
            services.AddSingleton<IDailyRunRepository, DailyRunRepository>();
            services.AddSingleton<IZiplingoEngagementRepository, ZiplingoEngagementRepository>();

            //Services
            services.AddSingleton<IDailyRunService, DailyRunService>();
            services.AddSingleton<IZiplingoEngagementService, ZiplingoEngagementService>();
            services.AddSingleton<ICommissionImportService, CommissionImportService>();


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

                options.AddHook<UpdateAssociateHook>();
                options.AddHook<WriteApplication>();
                options.AddHook<FinalizeAcceptedOrderHook>();
                options.AddHook<FinalizeNonAcceptedOrderHook>();
                options.AddHook<LogRealtimeRankAdvanceHook>();
                options.AddHook<MarkPackageShippedHook>();

            //Repositories
            services.AddSingleton<ICustomLogRepository, CustomLogRepository>();
            services.AddSingleton<IGenericReportRepository, GenericReportRepository>();
            services.AddSingleton<IReportSourceRepository, ReportSourceRepository>();

                // Merchants
            services.AddSingleton<IGenericReportService, GenericReportService>();
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddSwaggerGen();
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
            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V2");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    internal class WebExtensionTokenProvider : ITokenProvider
    {
        public string DirectScaleUrl { get; set; }
        public string DirectScaleSecret { get; set; }
        public string[] ExtensionSecrets { get; set; }

        async Task<string> ITokenProvider.GetDirectScaleSecret()
        {
            return await Task.FromResult(DirectScaleSecret);
        }

        async Task<string> ITokenProvider.GetDirectScaleServiceUrl()
        {
            return await Task.FromResult(DirectScaleUrl);
        }

        async Task<IEnumerable<string>> ITokenProvider.GetExtensionSecrets()
        {
            return await Task.FromResult(ExtensionSecrets);
        }
    }
}
