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

namespace TASMA
{
    /// <summary>
    /// 알람 메시지 박스 입니다.
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
            if (Application.Current.Windows.Count == 1)
            {
                this.Top = Application.Current.Windows[0].Top +
                    Application.Current.Windows[0].Height / 2 - Height / 2;
                this.Left = Application.Current.Windows[0].Left +
                    Application.Current.Windows[0].Width / 2 - Width / 2;
            }
            else
            {
                this.Top = Application.Current.Windows[1].Top + 
                    Application.Current.Windows[1].Height / 2 - Height / 2;
                this.Left = Application.Current.Windows[1].Left + 
                    Application.Current.Windows[1].Width / 2 - Width / 2;
            }
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
