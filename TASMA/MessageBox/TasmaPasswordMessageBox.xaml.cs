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
    /// Tasma 비밀번호 
    /// </summary>
    public partial class TasmaPasswordMessageBox : Window
    {
        private bool isDetermined = false;

        public bool IsDetermined
        {
            get { return isDetermined; }
            set { isDetermined = value; }
        }

        private string input = null;

        public string Input
        {
            get { return input; }
        }

        public TasmaPasswordMessageBox(string title, string message)
        {
            InitializeComponent();
            InitialRoutine(title, message);
        }

        public TasmaPasswordMessageBox(string title, string message, string defaultInput)
        {
            InitializeComponent();
            InitialRoutine(title, message);
            TasmaPasswordWindow_PasswordBox.Password = defaultInput;
        }

        private void InitialRoutine(string title, string message)
        {
            TasmaPasswordWindow_Title.Text = title;
            TasmaPasswordWindow_Message.Text = message;

            TasmaPasswordWindow_CloseButton.Click += OnCancelButtonClicked;
            TasmaPasswordWindow_OK.Click += OnOKButtonClicked;
            TasmaPasswordWindow_Cancel.Click += OnCancelButtonClicked;
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            input = TasmaPasswordWindow_PasswordBox.Password;
            isDetermined = true;
            Close();
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            isDetermined = false;
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

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnOKButtonClicked(null, null);
            }
        }
    }
}
