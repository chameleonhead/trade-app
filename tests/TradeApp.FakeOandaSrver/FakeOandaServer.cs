using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TradeApp.FakeOandaSrver
{
    public class FakeOandaWebHost : IWebHost
    {
        private readonly string URL_TO_LISTEN = "http://localhost:5000";
        public Uri BaseUri => new Uri(URL_TO_LISTEN);
        private IWebHost webHost;

        private static string ACCESS_TOKEN_GUID = Guid.NewGuid().ToString();
        private readonly FakeOandaContext context;
        public FakeOandaContext Context { get { return context; } }
        public int DefaultAccountId { get { return context.DefaultAccount.AccountId; } }
        public string DefaultAccessToken { get { return ACCESS_TOKEN_GUID; } }

        public FakeOandaWebHost()
        {
            context = new FakeOandaContext();
            this.webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(URL_TO_LISTEN)
                .ConfigureServices(services => services.Add(new ServiceDescriptor(typeof(FakeOandaContext), context)))
                .UseStartup<FakeOandaStartup>()
                .Build();
        }

        public IFeatureCollection ServerFeatures => webHost.ServerFeatures;

        public IServiceProvider Services => webHost.Services;

        public void Dispose()
        {
            webHost.Dispose();
        }

        public void Start()
        {
            webHost.Start();
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return webHost.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return webHost.StopAsync(cancellationToken);
        }
    }

    public class FakeOandaTestServer : TestServer
    {
        private static string ACCESS_TOKEN_GUID = Guid.NewGuid().ToString();
        private readonly FakeOandaContext context;
        public FakeOandaContext Context { get { return context; } }
        public int DefaultAccountId { get { return context.DefaultAccount.AccountId; } }
        public string DefaultAccessToken { get { return ACCESS_TOKEN_GUID; } }

        public FakeOandaTestServer() : this(new FakeOandaContext())
        {
        }

        public FakeOandaTestServer(FakeOandaContext context)
            : base(new WebHostBuilder()
                  .ConfigureServices(services => services.Add(new ServiceDescriptor(typeof(FakeOandaContext), context)))
                  .UseStartup<FakeOandaStartup>())
        {
            this.context = context;
        }
    }

    public class FakeOandaStartup
    {
        private readonly FakeOandaContext context;

        public FakeOandaStartup(FakeOandaContext context)
        {
            this.context = context;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<FakeOandaMiddleware>(context);
        }
    }
}