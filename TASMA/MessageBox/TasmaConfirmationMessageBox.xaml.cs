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
    /// Tasma 디자인의 확인 메세지 박스
    /// </summary>
    public partial class TasmaConfirmationMessageBox : System.Windows.Window
    {
        private bool yes = false;
        public bool Yes
        {
            get { return yes;  }
            set { yes = value; }
        }

        public TasmaConfirmationMessageBox(string title, string message)
        {
            InitializeComponent();
            TasmaConfirmation_Title.Text = title;
            TasmaConfirmation_Message.Text = message;
        }

        private void OnYesClicked(object sender, RoutedEventArgs e)
        {
            yes = true;
            Close();
        }

        private void OnNoClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Top = Application.Current.Windows[1].Top + Application.Current.Windows[1].Height / 2 - Height / 2;
            this.Left = Application.Current.Windows[1].Left + Application.Current.Windows[1].Width / 2 - Width / 2;
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
