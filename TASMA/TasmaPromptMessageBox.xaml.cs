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
    /// Tasma 디자인의 프롬프트 박스
    /// </summary>
    public partial class TasmaPromptMessageBox : Window
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

        public TasmaPromptMessageBox(string title, string message)
        {
            InitializeComponent();
            InitialRoutine(title, message);
        }

        public TasmaPromptMessageBox(string title, string message, string defaultInput)
        {
            InitializeComponent();
            InitialRoutine(title, message);
            TasmaPromptWindow_TextBox.Text = defaultInput;
        }

        private void InitialRoutine(string title, string message)
        {
            TasmaPromptWindow_Title.Text = title;
            TasmaPromptWindow_Message.Text = message;

            TasmaPromptWindow_CloseButton.Click += OnCancelButtonClicked;
            TasmaPromptWindow_OK.Click += OnOKButtonClicked;
            TasmaPromptWindow_Cancel.Click += OnCancelButtonClicked;
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            input = TasmaPromptWindow_TextBox.Text;
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

            if (TasmaPromptWindow_TextBox.Text.Length != 0)
            {
                TasmaPromptWindow_TextBox.Focus();
                TasmaPromptWindow_TextBox.Select(0, TasmaPromptWindow_TextBox.Text.Length);
            }
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
