using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WATERCryst.myBIOCAT.RestClient
{
    internal static class Api
    {
        public static void Main() {}
    }
    
    
    public class CallbackServer : IDisposable
    {
        private readonly ILogger _logger;
        private readonly RestClient _client;
        private readonly int _port;
        private readonly WebApplication _app;

        public CallbackServer(ILogger logger, RestClient client, int port)
        {
            _logger = logger;
            _client = client;
            _port = port;
            
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddSingleton(logger);
            builder.Services.AddSingleton(client);
            builder.Services.AddControllers();
            
            _app = builder.Build();

            if (_app.Environment.IsDevelopment())
            {
                _app.UseDeveloperExceptionPage();
            }

            _app.MapGet("/myBIOCAT", CallbackHandler);

            //_app.UseHttpsRedirection();
            _app.MapControllers();
        }

        private Task CallbackHandler(HttpContext context)
        {
            _logger.LogTrace("Callback");
            _client.OnCallback(context.Request.Body);
            return Task.CompletedTask;
        }

        public void Start()
        {
            _app.Run($"http://*: {_port}");
        }

        public void Dispose()
        {
            try
            {
                _app.StopAsync().Wait();
            }
            catch
            {
                // ignore
            }
        }
        
    }
}