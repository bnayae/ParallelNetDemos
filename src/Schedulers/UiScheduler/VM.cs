using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tpl.Samples;

namespace Sela.Samples
{
    public class VM 
    {
        private readonly TaskScheduler _scd;
        private readonly Timer _tmr;
        public VM()
        {
            _scd = TaskScheduler.FromCurrentSynchronizationContext(); 
            Data = new ObservableCollection<string>();
            _tmr = new Timer(state =>
                {
                    //Data.Add(Environment.TickCount);
                    Task.Factory.StartNew(() => Data.Insert(0, GetRandomString()),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        _scd); // execute on the UI thread
                }, null, 2000, 1000);
            Task.Run(() => Thread.Sleep(1000)) // do some off-line work
                .ContinueWith(completed =>
                    {
                        Data.Add("Enable 5 seconds task");

                        #region Remarked

                        // using the current Task Scheduler 
                        //(which happens to be the Synchronization Context Scheduler)
                        //Task.Factory.StartNew(() => Thread.Sleep(5000));

                        #endregion // Remarked

                        #region Remarked

                        // Force the default scheduler
                        //Task.Factory.StartNew(() => Thread.Sleep(10000),
                        //    CancellationToken.None,
                        //    TaskCreationOptions.None,
                        //    TaskScheduler.Default);
                        //Task.Run(() => Thread.Sleep(10000));

                        #endregion // Remarked
                    }, _scd);
        }

        private static string GetRandomString()
        {
            char c = (char)((int)'A' + (Environment.TickCount % 26));
            return c.ToString();
        }
        public ObservableCollection<string> Data { get; set; }
    }
}
