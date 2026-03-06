using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Ink;
using System.Windows.Navigation;
using System.Xml;


namespace tetris2022
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public System.Windows.Media.Color color ;
        public Window1()
        {
            InitializeComponent();
            updateColor();
        }

        private void RGBSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            updateColor();
        }
        public void updateColor()
        {
            byte red = (byte)RedSlider.Value;
            byte green = (byte)GreenSlider.Value;
            byte blue = (byte)BlueSlider.Value;

            fill.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(red,green,blue));
            this.color = System.Windows.Media.Color.FromRgb(red, green, blue);

        }

        private void save(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
