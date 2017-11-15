using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;

namespace TradeApp.FakeServer
{
    class HttpRequestListener
    {
        private BlockingCollection<HttpContext> requests;

        public HttpRequestListener()
        {
            requests = new BlockingCollection<HttpContext>();
        }

        public void Dispatch(HttpContext context)
        {
            requests.Add(context);
        }

        public bool Process(Action<HttpContext> processor)
        {
            var context = default(HttpContext);
            if (requests.TryTake(out context, TimeSpan.FromSeconds(5)))
            {
                processor.Invoke(context);
                return true;
            }
            return false;
        }
    }
}
