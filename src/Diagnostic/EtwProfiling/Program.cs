#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace EtwProfiling
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            //Parallel.For(0, 10, i => Thread.Sleep(1));

            #region Remarked

            CustomEtw.Log.Startup();
            Thread.Sleep(1);

            string fileName = Guid.NewGuid().ToString("N") + ".txt";
            CustomEtw.Log.IOStart(1, fileName);
            Task t = Write(fileName);

            Console.WriteLine("Writing");

            t.Wait();
            CustomEtw.Log.IOStop(1);

            #endregion // Remarked

            Console.WriteLine("Complete");
        }

        private static async Task Write(string fileName)
        {
            try
            {
                var data = new string('*', 100000);
                var options = FileOptions.Asynchronous | FileOptions.WriteThrough;
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, options))
                //using (var fs = File.OpenWrite(fileName))
                using (var sw = new StreamWriter(fs))
                {
                    await sw.WriteAsync(data);
                }
            }
            catch (Exception ex)
            {
                //CustomEtw.Log.Failure(ex.Message);
            }
        }
    }
}
