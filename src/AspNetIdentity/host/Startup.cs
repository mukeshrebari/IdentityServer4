using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServerHost.Data;
using IdentityServerHost.Configuration;
using IdentityServer4.Models;
using Microsoft.Extensions.Hosting;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Identity;

namespace IdentityServerHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
             services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), dbOpts => dbOpts.MigrationsAssembly(typeof(Startup).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();

            services.AddIdentityServer()
                //.AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
                //.AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(IdentityServerHost.Configuration.Resources.IdentityResources)
                .AddInMemoryApiResources(IdentityServerHost.Configuration.Resources.ApiResources)
                .AddInMemoryApiScopes(IdentityServerHost.Configuration.Resources.ApiScopes)
                .AddInMemoryClients(Clients.Get())
                .AddAspNetIdentity<ApplicationUser>();
            services.Configure<IdentityOptions>(options =>
            {
                // Configure your Identity options here
                options.SignIn = new SignInOptions
                {
                    RequireConfirmedAccount = false,
                    RequireConfirmedEmail = false,
                    RequireConfirmedPhoneNumber = false
                };
            });
            services.AddAuthentication()
                  //.AddIdentityServerJwt()
                .AddOpenIdConnect("Google", "Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

                    options.Authority = "https://accounts.google.com/";
                    options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";

                    options.CallbackPath = "/signin-google";
                    options.Scope.Add("email");
                });
        }
        //write a method which decides if the new consignment can be loaded into the truck or not in accordance to truck's capacity in the form of Volume and Weight

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapRazorPages();
            });
        }
    }
}