using System;
using System.Collections.Generic;
using System.Data;
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
using TASMA.Database;

namespace TASMA.Window
{
    /// <summary>
    /// PrintDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PrintDialog : System.Windows.Window
    {
        private AdminDAO adminDAO;
        private DataTable selectedData; 

        public PrintDialog(AdminDAO adminDAO, DataTable selectedData)
        {
            InitializeComponent();
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
