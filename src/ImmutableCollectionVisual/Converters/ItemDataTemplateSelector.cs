using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Sela.Samples
{
    public class ItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (item is IBinaryTree<int>)
                return element.FindResource("NodeTemplate") as DataTemplate;
            return element.FindResource("ConnectorTemplate") as DataTemplate;
        }
    }
}
