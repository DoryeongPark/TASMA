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

namespace TASMA.MessageBox
{
    /// <summary>
    /// 알림 메시지 박스 입니다.
    /// </summary>
    public partial class TasmaAlertMessageBox: System.Windows.Window
    {
        public TasmaAlertMessageBox(string title, string message)
        {
            InitializeComponent();

            TasmaAlert_Title.Text = title;
            TasmaAlert_Message.Text = message;
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
