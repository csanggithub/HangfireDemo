using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MySql.Core;
using HangfireDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HangfireDemo
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
            var mySqlConnString = Configuration.GetConnectionString("MySqlConnString");

            services.AddControllers();
            #region Hangfire
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(mySqlConnString, new MySqlStorageOptions() { TablePrefix = "hf_" })));
            #endregion

            #region 注册接口
            services.AddTransient<IHangfireServices, HangfireServices>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Hangfire
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1 //允许进程链接数量 
            });
            app.UseHangfireDashboard("/hfire", new DashboardOptions
            {
                DisplayStorageConnectionString = false,
                AppPath = "/" //返回的url
            });
            RecurringJob.AddOrUpdate<IHangfireServices>(x => x.TaskOne(), "0/10 * * * *");
            RecurringJob.AddOrUpdate<IHangfireServices>(x => x.TaskTwo(), Cron.Minutely);
            #endregion

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
