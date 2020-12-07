using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeachGroveApartments.ApplicationLayer.HelperClasses;
using PeachGroveApartments.ApplicationLayer.Interfaces;
using PeachGroveApartments.Core.Interfaces;
using PeachGroveApartments.Infrastructure.Data;
using PeachGroveApartments.Infrastructure.Identity;
using PeachGroveApartments.Infrastructure.Interfaces;
using System;

namespace Peach_Grove_Apartments_Demo_Project
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = null;
            connectionString = CurrentEnvironment.IsDevelopment() ? "DefaultConnection" : "VPSConnection";

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString(connectionString)));
            services.AddIdentity<AptUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })

            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI()
            .AddRoles<IdentityRole>();

            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IHomeRepository, HomeRepository>();
            services.AddScoped<IApplicantRepository, ApplicantRepository>();
            services.AddScoped<IResidentRepository, ResidentRepository>();
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<IManagerAccountLogic, ManagerAccountLogic>();
            services.AddScoped<IResidentAccountLogic, ResidentAccountLogic>();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddAutoMapper(c => c.AddProfile<AutoMappingProfile>(), typeof(Startup));
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }
    }
}