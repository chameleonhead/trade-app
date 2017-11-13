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
            int retryCount = 0;
            while (retryCount++ < 5)
                if (requests.TryTake(out context, TimeSpan.FromSeconds(1)))
                {
                    processor.Invoke(context);
                    return true;
                }
            return false;
        }
    }
}
