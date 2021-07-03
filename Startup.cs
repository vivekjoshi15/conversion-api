using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using conversion_api.Models;
using conversion_api.Utilities;

namespace conversion_api
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            IConfigurationSection sectionEmailProvider = Configuration.GetSection("EmailProvider");
            string emailProvider = sectionEmailProvider["Provider"];

            string emailConnectionType = sectionEmailProvider["ConnectionType"];

            if (null == emailProvider)
            {
                throw new ArgumentNullException(
                    "EmailProvider",
                    "Missing email service provider configuration"
                );
            }

            IConfiguration emailConfig = LoadEmailConfig();

            switch (emailProvider.ToLower())
            {
                case "mailgun":
                    Console.WriteLine(
                        "Starting send mail via MailGun email service...");
                    
                    if ("smtp" == emailConnectionType.ToLower())
                    {

                        services.AddTransient<IEmailSender,
                            MailGunSmtpEmailSender>();

                        services.Configure<MailGunSmtpEmailSettings>(
                            emailConfig);

                    }
                    break;
                default:
                    Console.WriteLine("Error: Unknown email service.");
                    throw new ArgumentException(
                        "Unknown email service provider in configuration.",
                        "EmailProvider:Provider"
                    );
                    // break;
            }

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );

            services.AddSession(options =>
            {
                options.Cookie.Name = ".Conversion.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<DBContext>(options => options.UseMySql(Configuration.GetConnectionString("conversionDatabase"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.20-mysql")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected IConfiguration LoadEmailConfig()
        {

            IConfigurationSection sectionEmailProvider =
                Configuration.GetSection("EmailProvider");

            string emailProvider = sectionEmailProvider["Provider"];

            if (null == emailProvider)
            {
                throw new ArgumentException(
                    "Empty email provider.", "EmailProvider:Provider"
                );
            }

            IConfiguration emailConfig = null;
            switch (emailProvider.ToLower())
            {
                case "mailgun":
                    Console.WriteLine("Loading MailGun configuration...");

                    emailConfig = LoadMailGunEmailConfig();

                    break;
                default:
                    throw new ArgumentException(
                        "Unknown email provider.",
                        "EmailProvider:Provider"
                    );
                    // break;
            }
            return emailConfig;
        }

        protected IConfiguration LoadMailGunEmailConfig()
        {

            IConfigurationSection sectionEmailProvider =
                Configuration.GetSection("EmailProvider");

            string emailConnectionType =
                sectionEmailProvider["ConnectionType"];

            IConfiguration emailConfig = null;

            if (null == emailConnectionType)
            {

                throw new ArgumentException(
                    "Invalid email connection type.",
                    "EmailProvider:ConnectionType"
                );

            }
            switch (emailConnectionType.ToLower())
            {
                case "api":
                    emailConfig = LoadMailGunApiEmailSettings();
                    break;
                case "smtp":
                    emailConfig = LoadMailGunSmtpEmailSettings();
                    break;

                default:

                    throw new ArgumentException(
                        "Unknown email connection type.",
                        "EmailProvider:ConnectionType"
                    );

                    // break;
            }
            return emailConfig;
        }

        //
        // Configure email sender based on Mailgun configurations for REST API.
        // 
        protected IConfiguration LoadMailGunSmtpEmailSettings()
        {

            IConfigurationSection sectionEmailProvider =
                Configuration.GetSection("EmailProvider");

            string configFile = sectionEmailProvider["ConfigFile"];

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            IConfigurationSection sectionSmtp = config.GetSection("SMTPSettings");
            return sectionSmtp;
        }

        //
        // Configure email sender based on Mailgun configurations for Smtp.
        // 
        protected IConfiguration LoadMailGunApiEmailSettings()
        {
            IConfigurationSection sectionEmailProvider =
                Configuration.GetSection("EmailProvider");

            string configFile = sectionEmailProvider["ConfigFile"];

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            IConfigurationSection sectionRestApi =
                config.GetSection("RestAPISettings");

            return sectionRestApi;

        }
    }
}
