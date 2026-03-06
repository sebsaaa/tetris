using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace tetris2022
{
    /// <summary>
    /// Interaction logic for FigurePicker.xaml
    /// </summary>
    public partial class FigurePicker : Window
    {
        public FigurePicker(List<System.Windows.Media.Color> colors)
        {
            InitializeComponent();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++) {
                    coordList[x,y] = false;
                }
            }
            Random rand = new Random();
            color = colors[rand.Next(0, colors.Count - 1)];

            for (int x = 0; x < 12; x++)
            {

               timesClicked[x] = 0;

            }
        }

        public System.Windows.Media.Color color;
        public bool[,] coordList = new bool[3,4];
        public int[] timesClicked = new int[12];

        private void buttonClicked(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int tag = Convert.ToInt16(clickedButton.Tag);
            timesClicked[tag-1]++;
            int y = (tag -1) / 3;
            int x = (tag -1) %3;
            coordList[x, y] = true;
            if (timesClicked[tag-1] % 2 == 1) {
                clickedButton.Background = new LinearGradientBrush(
                   System.Windows.Media.Color.FromArgb(color.A, (byte)Math.Min(this.color.R + 180, 255), (byte)Math.Min(this.color.G + 180, 255), (byte)Math.Min(this.color.B + 180, 255)),
                   System.Windows.Media.Color.FromArgb(color.A, this.color.R, this.color.G, this.color.B),
                   45
                   );
                clickedButton.BorderBrush = new LinearGradientBrush(
                        System.Windows.Media.Color.FromArgb(color.A, this.color.R, this.color.G, this.color.B),
                        System.Windows.Media.Color.FromArgb(color.A, (byte)(this.color.R * 0.8), (byte)(this.color.G * 0.8), (byte)(this.color.B * 0.8)),
                        45
                        );
                clickedButton.BorderThickness = new Thickness(1.5);
            }
            else
            {
                clickedButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(2, 22, 40));
                clickedButton.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 51, 110));
                clickedButton.BorderThickness = new Thickness(1);

                coordList[x, y] = false;

            }
            
        }

        private void save(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
