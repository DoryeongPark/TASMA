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
    /// Tasma 디자인 프롬프트 창
    /// </summary>
    public partial class TasmaPromptWindow : Window
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

        public TasmaPromptWindow(string title, string message)
        {
            InitializeComponent();
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
            this.Top = Application.Current.Windows[1].Top + Application.Current.Windows[1].Height / 2;
            this.Left = Application.Current.Windows[1].Left + Application.Current.Windows[1].Width / 2;
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
