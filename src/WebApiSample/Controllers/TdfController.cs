using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web.Http;

namespace WebApiSample.Controllers
{
    // http://localhost:5287/api/tdf?input=3
    public class TdfController : ApiController
    {
        private static ActionBlock<AsyncMessage<int, string>> _asyncProcessor =
            new ActionBlock<AsyncMessage<int, string>>(
            async message =>
            {
                try
                {
                    await Task.Delay(2000); // IO
                    string result = new string('*', message.Input);
                    message.Complete(result);
                }
                catch (Exception ex)
                {
                    message.Fault(ex);
                }
            });

        public Task<string> Get(int input)
        {
            var message = new AsyncMessage<int, string>(input);
            if (!_asyncProcessor.Post(message))
                throw new OperationCanceledException();
            return message.Completion;
        }

        private class AsyncMessage<TIn, TOut>
        {
            private readonly TaskCompletionSource<TOut> _tcs = new TaskCompletionSource<TOut>();
            public readonly TIn Input;
            public AsyncMessage(TIn input)
            {
                Input = input;
            }
            public Task<TOut> Completion => _tcs.Task;
            public void Complete(TOut result) => _tcs.TrySetResult(result);
            public void Fault(Exception ex) => _tcs.TrySetException(ex);
        }
    }
}
