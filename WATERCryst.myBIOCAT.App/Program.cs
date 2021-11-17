// See https://aka.ms/new-console-template for more information

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace WATERCryst.myBIOCAT.App
{
    public static class Program
    {
        private static void Main()
        {
            Console.WriteLine("myBIOCAT");

            var configureNamedOptions = new ConfigureNamedOptions<ConsoleLoggerOptions>("", null);
            var optionsFactory = new OptionsFactory<ConsoleLoggerOptions>(new []{ configureNamedOptions }, Enumerable.Empty<IPostConfigureOptions<ConsoleLoggerOptions>>());
            var optionsMonitor = new OptionsMonitor<ConsoleLoggerOptions>(optionsFactory, Enumerable.Empty<IOptionsChangeTokenSource<ConsoleLoggerOptions>>(), new OptionsCache<ConsoleLoggerOptions>());
            var loggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider(optionsMonitor) }, new LoggerFilterOptions { MinLevel = LogLevel.Information });
            var logger = loggerFactory.CreateLogger("myBIOCAT");            
            
            var apiKey = Environment.GetEnvironmentVariable("myBIOCAT") ?? "";
            using var client = new RestClient.RestClient(logger, apiKey);
    
            var state = client.GetCurrentDeviceState();
            Console.WriteLine(state?.Online);
            
            Console.ReadLine();
            Console.WriteLine("done.");
        }
    
    }
}
