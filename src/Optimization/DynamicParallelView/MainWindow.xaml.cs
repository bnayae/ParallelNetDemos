#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.ComponentModel;

#endregion // Using

namespace Tpl.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private / Protected Fields

        private const int SPIN_DURATION = 20;
        private ParallelViewModel _viewModel;

        #endregion // Private / Protected Fields

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new ParallelViewModel();
            this.DataContext = _viewModel;
        }

        #endregion // Constructors

        #region Data

        private IEnumerable<int> Data 
        {
            get{ return  Enumerable.Range(0, _viewModel.ItemsPerCore * Environment.ProcessorCount);}
        }

        #endregion // Data
    }
}
