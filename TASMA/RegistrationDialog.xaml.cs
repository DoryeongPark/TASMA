using System;
using System.Collections.Generic;
using System.IO;
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

namespace TASMA
{
    /// <summary>
    /// 선생님 계정 등록 다이얼로그
    /// </summary>
    public partial class RegistrationDialog : Window
    {
        private bool isDetermined = false;

        public bool IsDetermined
        {
            get { return isDetermined; }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
        }

        private string password;

        public string Password
        {
            get { return password;  }
        }

        private string confirmPassword;
        
        public RegistrationDialog()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
        }

       
        private void OnClickOKButton(object sender, RoutedEventArgs e)
        {

            userName = RegistrationDialog_Username.Text;
            password = RegistrationDialog_Password.Password;
            confirmPassword = RegistrationDialog_ConfirmPassword.Password;
            
            if(userName == "")
            {
                MessageBox.Show("You should input Username");
                return;
            }

            if(password == "")
            {
                MessageBox.Show("You should input Password");
                return;
            }

            if(confirmPassword == "")
            {
                MessageBox.Show("You should input Confirm Password");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords are not matching");
                return;
            }

            if(new FileInfo(userName + ".db").Exists)
            {
                MessageBox.Show("ID already exists");
                return;
            }

            isDetermined = true;
            Close();
        }

        private void OnClickCancelButton(object sender, RoutedEventArgs e)
        {
            isDetermined = false;
            Close();
        }
    }
}
