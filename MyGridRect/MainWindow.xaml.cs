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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridRect
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            gfs.SelectChildChange += Gfs_SelectChildChange;
            dropShadowEffect.Color = Colors.SkyBlue;
            dropShadowEffect.ShadowDepth = -4;
            dropShadowEffect.BlurRadius = 5;
        }

        DropShadowEffect dropShadowEffect = new DropShadowEffect();

        private void Gfs_SelectChildChange(object sender, RoutedEventArgs e)
        {
            if (e is SelectedChangedEventArgs args)
            {
                foreach (var item in args.SelectedControls)
                {
                    if (item is CheckBox cb)
                    {
                        cb.SetValue(EffectProperty, dropShadowEffect);
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.gfs.Children)
            {
                if (item is CheckBox cb)
                {
                    cb.SetValue(CheckBox.IsCheckedProperty, true);
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.gfs.Children)
            {
                if (item is CheckBox cb)
                {
                    cb.SetValue(CheckBox.IsCheckedProperty, false);
                }
            }
        }
    }
}
