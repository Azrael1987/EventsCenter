using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace Evento.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try  // NLog: setup the logger first to catch all errors
            {
                logger.Debug("init main");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)  //NLog: catch setup errors
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {  // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(
                    hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            })
            .UseStartup<Startup>()
           .ConfigureLogging(loggingForNLog =>
           {
               loggingForNLog.ClearProviders();
               loggingForNLog.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
           })
            .UseNLog(); // NLog: setup NLog for Dependency injection
    }
}
