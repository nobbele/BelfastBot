using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Discord;
using YohaneBot;
using YohaneBot.Services.Database;
using YohaneBot.Services.Logging;
using YohaneBot.Services.Scheduler;
using YohaneBot.Services.Pagination;
using YohaneBot.Services.Credits;
using YohaneBot.Services.Giveaway;
using System.Net.Http;
using YohaneBot.Services.Commands;
using Discord.Commands;
using YohaneBot.Services.Configuration;
using YohaneBot.Services.Communiciation;

namespace YohaneWebClient
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
            services.AddRazorPages();
            services.AddSingleton<IDiscordClient, WebFakeDiscordSocketClient>()
                .AddSingleton<IBotConfigurationService, BotConfigurationService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<WebYohaneClient>()
                    .AddSingleton<IClient>(coll => coll.GetRequiredService<WebYohaneClient>())
                .AddSingleton<WebCommunicationService>()
                .AddSingleton<ICommunicationService>(coll => coll.GetRequiredService<WebCommunicationService>())
                .AddSingleton<JsonDatabaseService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<SchedulerService>()
                .AddSingleton<PaginatedMessageService>()
                .AddSingleton<MessageRewardService>()
                .AddSingleton<GiveawayService>()
                .AddSingleton<DummyType>()
                .AddSingleton<CommandService>()
                .AddSingleton<WebCommandHandlingService>()
                .AddSingleton<IServiceProvider>(coll => coll);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("api", "{controller}/{action}");
            });

            app.ApplicationServices.GetRequiredService<WebCommandHandlingService>().Initialize();
        }
    }
}
