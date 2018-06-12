using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _170_AsyncThreadLocal
{
    public class NonThreadSafeClient : IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();

        public void Dispose()
        {
        }

        public string Read()
        {
            Console.WriteLine(Id);
            return null;
        }

        internal void Write(string v)
        {
            Console.WriteLine(Id);
        }
    }
}
