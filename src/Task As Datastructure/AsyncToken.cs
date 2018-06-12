using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_As_Datastructure
{
    public class AsyncToken
    {
        private readonly string _value;
        private readonly TaskCompletionSource<string> _tcs;

        public AsyncToken(string value)
        {
            _tcs = new TaskCompletionSource<string>();
            _value = value;
        }
        public AsyncToken(string value, AsyncToken continueFrom)
        {
            _value = value;
            _tcs = continueFrom._tcs;
        }

        public void TryComplete()
        {
            _tcs.TrySetResult(_value);
        }

        public Task<string> Completion => _tcs.Task;
        //public Task<int> Completion { get { return _tcs.Task; } }

        public override string ToString() => _value;
    }
}
