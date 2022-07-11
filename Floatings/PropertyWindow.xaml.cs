using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OverTop.Floatings
{
    /// <summary>
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyWindow : Window
    {
        public PropertyWindow()
        {
            InitializeComponent();
        }

        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App.parameterClass.alpha = AlphaSlider.Value;
            alphaTextBlock.Text = AlphaSlider.Value.ToString();
        }
    }
}
