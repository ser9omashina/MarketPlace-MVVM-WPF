using MarketPlace.ViewModel;
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

namespace MarketPlace.View
{
    /// <summary>
    /// Логика взаимодействия для LoaderOrManagerWindow.xaml
    /// </summary>
    public partial class LoaderOrManagerWindow : Window
    {
        public LoaderOrManagerWindow()
        {
            InitializeComponent();
            DataContext = new LoaderOrManagerModel();
        }
    }
}
