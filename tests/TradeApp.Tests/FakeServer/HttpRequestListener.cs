using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TradeApp.FakeServer
{
    class HttpRequestListener
    {
        private BlockingCollection<Tuple<TaskCompletionSource<object>, HttpContext>> requests;

        public HttpRequestListener()
        {
            requests = new BlockingCollection<Tuple<TaskCompletionSource<object>, HttpContext>>();
        }

        public async Task Dispatch(HttpContext context)
        {
            var tcs = new TaskCompletionSource<object>();
            requests.Add(Tuple.Create(tcs, context));
            await tcs.Task;
        }

        public void Process(Action<HttpRequest, HttpResponse> processor, TimeSpan wait)
        {
            var value = default(Tuple<TaskCompletionSource<object>, HttpContext>);
            if (requests.TryTake(out value, wait))
            {
                var context = value.Item2;
                processor.Invoke(context.Request, context.Response);
                value.Item1.SetResult(default(object));
            }
        }
    }
}
