using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtBearMemoryLeak.Authentications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JwtBearMemoryLeak
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            AddApplicationServices(services);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
        }

        private void AddApplicationServices(IServiceCollection services)
        {
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, CustomJwtBearerOptions>();
            services.AddScoped<IUserInfo, UserInfo>();
            services.AddSingleton<ICredentialProvider, RsaCredentialProvider>();
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

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            
            app.UseMvc();
        }
    }
}
