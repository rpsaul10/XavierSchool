using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XavierSchoolMicroService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog ((context, config) =>
                {
                    config.WriteTo.Console ();
                    config.WriteTo.File ("Logs.txt", Serilog.Events.LogEventLevel.Information);
                    config.WriteTo.ApplicationInsights(new TelemetryClient ()
                    {
                        InstrumentationKey = "f893f05a-e09b-43ec-b3a0-e7e917b0e56b",
                    }, TelemetryConverter.Events);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
