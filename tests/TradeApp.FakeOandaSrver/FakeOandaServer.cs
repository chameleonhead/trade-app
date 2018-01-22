using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TradeApp.FakeOandaSrver
{
    public class FakeOandaServer : TestServer
    {
        private static string ACCESS_TOKEN_GUID = Guid.NewGuid().ToString();

        private readonly FakeOandaContext context;

        public FakeOandaServer() : this(new FakeOandaContext())
        {
        }

        public FakeOandaServer(FakeOandaContext context)
            : base(new WebHostBuilder()
                  .ConfigureServices(services => services.Add(new ServiceDescriptor(typeof(FakeOandaContext), context)))
                  .UseStartup<FakeOandaStartup>())
        {
            this.context = context;
        }

        public FakeOandaContext Context { get { return context; } }
        public int DefaultAccountId { get { return context.DefaultAccount.AccountId; } }
        public string DefaultAccessToken { get { return ACCESS_TOKEN_GUID; } }
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