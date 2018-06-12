using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurrentVsDefaultScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private ObservableCollection<string> _items = new ObservableCollection<string>();

        public ObservableCollection<string> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        private void OnClickHandler(object sender, RoutedEventArgs e)
        {
            TaskScheduler scd = TaskScheduler.FromCurrentSynchronizationContext();
            Task<string[]> t1 = Task.Run(() =>
                {
                    Thread.Sleep(4000);
                    return Enumerable.Range(1, 20)
                        .Select(v => new string('*', v))
                        .ToArray(); // return a string[]
                });

            #region Option 1

            t1.ContinueWith(t =>
                {
                    foreach (var item in t.Result)
                    {
                        _items.Add(item);
                    }

                    // use Task.Run
                    Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(10000); // should be async
                        });
                }, scd);

            #endregion // Option 1

            #region Option 2

            //t1.ContinueWith(t =>
            //    {
            //        foreach (var item in t.Result)
            //        {
            //            _items.Add(item);
            //        }

            //        // use Task.Run
            //        Task.Factory.StartNew(() =>
            //            {
            //                Thread.Sleep(10000); // should be async
            //            });
            //    }, CancellationToken.None, TaskContinuationOptions.HideScheduler, scd);

            #endregion // Option 2

            #region !option 3

            //t1.ContinueWith(t =>
            //    {
            //        foreach (var item in t.Result)
            //        {
            //            _items.Add(item);
            //        }

            //        // use Task.Run
            //        Task.Factory.StartNew(() =>
            //            {
            //                Thread.Sleep(10000); // should be async
            //            }, TaskCreationOptions.HideScheduler);
            //    }, scd);

            #endregion // !option 3

            #region Option 4

            //t1.ContinueWith(t =>
            //    {
            //        foreach (var item in t.Result)
            //        {
            //            _items.Add(item);
            //        }

            //        // use Task.Run
            //        Task.Run(() =>
            //            {
            //                Thread.Sleep(10000); // should be async
            //            });
            //    }, scd);

            #endregion // Option 4
        }
    }
}
