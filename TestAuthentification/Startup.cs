using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;
using TestAuthentification.Services;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;

namespace TestAuthentification
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:5001",
                    ValidAudience = "http://localhost:5001",
                    ClockSkew = TimeSpan.Zero,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A5DeveloppeurSecureKey"))
                };
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<BookYourCarContext>(options => options.UseMySql("server=mvinet.fr;port=3306;database=BookYourCar;uid=a5d;password=pwtk@[gh$!7Z#&wX"));
            //Scaffold-DbContext "server=mvinet.fr;port=3306;database=a5d;uid=a5d;password=pwtk@[gh$!7Z#&wX" Pomelo.EntityFrameworkCore.Mysql -OutputDir Models -f
            // commande pour mettre à jour le contexte en fonction de la BDD
            services.AddIdentityCore<User>().AddErrorDescriber<CustomIdentityErrorDescriber>();

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BookYourCar", Version = "v1" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
            app.UseCors("EnableCORS");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();

            Environment.SetEnvironmentVariable("DomaineName", "Https://bookyourcar.tk/changePassword/");
            Environment.SetEnvironmentVariable("KeyAPIEmail", "8c8039f810dde01b9c8587d95a10b633");
            Environment.SetEnvironmentVariable("SecretAPIEmail", "7414cc9223d0a77e98573dba18c36fe7");
#if DEBUG
            Environment.SetEnvironmentVariable("UrlVerifEmail", "https://localhost:5001/api/auth/VerifEmail/");
#else
            //Environment.SetEnvironmentVariable("UrlResetPassword", https://bookyourcar.tk/changePassword/");
            Environment.SetEnvironmentVariable("UrlVerifEmail", "https://a5d-dotnet.mvinet.fr/api/auth/VerifEmail/");
#endif



        }
    }
}
